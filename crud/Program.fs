open System.Net
open System.Text.Json.Serialization

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection

let routeHandlers : HttpFunc -> HttpContext -> HttpFuncResult =
  choose [
    GET >=> choose [
      route "/" >=> setStatusCode 200
      route "/health" >=>
        match Database.health with
        | Ok () -> text "alive and up"
        | Error err -> setStatusCode 500 >=> text $"internal server error: {err}"
      route "/hello" >=> json (dict [ "Hello", "World" ])
    ]
    POST >=> choose [
      route "/echo" >=> text "echo"
      route "/user" >=>
        // saving a NEW user
        setStatusCode 501 // not implemented
    ]
    PUT >=> choose [
    ]
    PATCH >=> choose [
      routef "/user/%i"
        // modify attributes on a user
        (fun id -> setStatusCode 501 >=> text ($"patch user {id}")) // not implemented
    ]
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
      .UseKestrel(fun options -> options.Listen(IPAddress.Any, 5000))
      .Configure(fun app -> app.UseGiraffe routeHandlers)
      .ConfigureServices(configureServices)
      .Build()

  try
      host.Run()
      0
  with ex -> 1