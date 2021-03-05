open System.Net
open System.Text.Json.Serialization

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

let routeHandlers : HttpFunc -> HttpContext -> HttpFuncResult =
  choose [
    // GETs should never modify any state.
    // GETs should therefore also be idempotent.
    GET >=> choose [
      route "/" >=> setStatusCode 200
      route "/health" >=>
        match Database.health with
        | Ok () -> text "alive and up"
        | Error err -> setStatusCode 500 >=> text "internal server error, check logs for details."
      route "/hello" >=> json (dict [ "Hello", "World" ])
    ]
    POST >=> choose [
      route "/echo" >=> text "echo" // TODO: echo back POST body
    ]
    // PUTs are supposed to be idempotent.
    PUT >=> choose [
      route "/user" >=>
        // saving a new user
        setStatusCode 501 // not implemented
    ]
    PATCH >=> choose [
      routef "/user/%i"
        // modify attributes on a user
        (fun id -> setStatusCode 501 >=> text ($"patch user {id}")) // not implemented
    ]
    // DELETEs should be idempotent.
    DELETE >=> choose [
      route "/user" >=>
        // delete a (presumably existing) user
        setStatusCode 501 // not implemented
    ]
    setStatusCode 404 >=> text "not found my friend"
  ]

let configureApp (app: IApplicationBuilder) = app.UseGiraffe routeHandlers

let configureServices (services: IServiceCollection) =
    services.AddGiraffe() |> ignore

    let serializationOptions = SystemTextJson.Serializer.DefaultOptions
    serializationOptions.Converters.Add(JsonFSharpConverter(JsonUnionEncoding.FSharpLuLike))

    services.AddSingleton<Json.ISerializer>(SystemTextJson.Serializer(serializationOptions))
    |> ignore

[<EntryPoint>]
let main args =
  let host =
    WebHostBuilder()
      .UseKestrel()
      .UseKestrel(fun options -> options.Listen(IPAddress.Any, 8080))
      // Kestrel HTTP2 is not yet available on MacOS!
      // .ConfigureKestrel(fun options -> options.Listen(IPAddress.Any, 8080, (fun lo -> lo.Protocols <- HttpProtocols.Http2)))
      .Configure(fun app -> app.UseGiraffe routeHandlers)
      .ConfigureServices(configureServices)
      .Build()

  try
      host.Run()
      0
  with ex -> 1
