open System.Net
open System.Text.Json.Serialization

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection


let putNewUser (claims : AuthMockup.Claims) : HttpHandler =
  setStatusCode 501 // not implemented

let routeHandlers : HttpHandler =
  choose [
    // ------------------------------------------------------------------------
    // Examples
    //
    // echo back the request body
    // bindJson will give a 500 error if the JSON is invalid.
    POST >=> route "/echo" >=> bindJson (fun bla -> Successful.OK bla)
    // a plain dict will do for json output
    GET >=> route "/example-hello" >=> json (dict [ "Hello", "World" ])
    // ------------------------------------------------------------------------

    // GETs should never modify any state.
    // GETs should therefore also be idempotent.
    GET >=> choose [
      route "/" >=> Successful.OK "hi there, welcome to my API 🌸"
      route "/health" >=>
        match Database.health with
        | Ok _ -> text "API and database are alive and up"
        | Error err -> setStatusCode 500
    ]
    POST >=> choose [
    ]
    // PUTs are supposed to be idempotent.
    PUT >=> choose [
      route "/user" >=>
        (AuthMockup.ensureClaims putNewUser)
        // setStatusCode 501
    ]
    PATCH >=> choose [
      routef "/user/%i"
        // modify attributes on an existing user
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
      .Configure(configureApp)
      .ConfigureServices(configureServices)
      .Build()
  try
      host.Run()
      0
  with ex -> 1
