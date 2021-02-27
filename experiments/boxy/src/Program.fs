open System
open System.IO
open System.Net
open System.Text.Json
open System.Text.Json.Serialization
open FSharp.SystemTextJson

open Giraffe
// open Giraffe.Serialization
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection



// let jsonOptions = JsonSerializerOptions()
// jsonOptions.Converters.Add(JsonFSharpConverter())

// JsonSerializer.Serialize({| x = "Hello"; y = "world!" |}, jsonOptions)

[<JsonFSharpConverter>]
type Example = { x: string; y: string list }

let routeHandlers: HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ GET
             >=> choose [ route "/" >=> setStatusCode 200
                          route "/info"
                          >=> json ({ x = "Hello"; y = [ "world!" ] }) ]
             setStatusCode 404 >=> text "Not Found" ]

let configureApp (app: IApplicationBuilder) = app.UseGiraffe routeHandlers

let configureServices (services: IServiceCollection) =
    // First register all default Giraffe dependencies
    services.AddGiraffe() |> ignore

    let serializationOptions = SystemTextJson.Serializer.DefaultOptions
    // Optionally use `FSharp.SystemTextJson` (requires `FSharp.SystemTextJson` package reference)
    serializationOptions.Converters.Add(JsonFSharpConverter(JsonUnionEncoding.FSharpLuLike))
    // Now register SystemTextJson.Serializer
    services.AddSingleton<Json.ISerializer>(SystemTextJson.Serializer(serializationOptions))
    |> ignore
// services.AddGiraffe() |> ignore

// let jsonOptions = JsonSerializerOptions()
// jsonOptions.Converters.Add(JsonFSharpConverter())
// services.AddSingleton<Json.ISerializer>(SystemTextJson.Serializer(jsonOptions)) |> ignore

// let serializationOptions = SystemTextJson.Serializer.DefaultOptions
// // Optionally use `FSharp.SystemTextJson` (requires `FSharp.SystemTextJson` package reference)
// serializationOptions.Converters.Add(JsonFSharpConverter())
// // Now register SystemTextJson.Serializer
// services.AddSingleton<Json.ISerializer>(SystemTextJson.Serializer(serializationOptions)) |> ignore


// let configureServices (services : IServiceCollection) =
//     services.AddGiraffe() |> ignore

[<EntryPoint>]
let main args =
    // we set the environment variables directly for the
    // case that we're running in a debug session in the editor.
    Config.setVariablesFrom ".env"

    let dummy =
        Environment.GetEnvironmentVariable "DUMMY_VARIABLE"

    let bummy =
        Environment.GetEnvironmentVariable "BUMMY_VARIABLE"

    //    let config =
//        ConfigurationBuilder()
//            .AddCommandLine(args)
//            // .AddEnvironmentVariables("ASPNETCORE_")
//            .Build()

    let host =
        WebHostBuilder()
            .UseKestrel(fun options -> options.Listen(IPAddress.Loopback, 5080))
            .Configure(fun app -> app.UseGiraffe routeHandlers)
            .ConfigureServices(configureServices)
            // .ConfigureLogging(configureLogging)
            .Build()

    try
        host.Run()
        0
    with ex ->
        // Log.Emergency("Host terminated unexpectedly.", ex)
        1
