open System.Net
open System.Text.Json.Serialization

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection


let routeHandlers : HttpHandler =
  choose [
    GET >=> route "/" >=> Successful.OK "hi there, welcome to my API 🌸"

    GET >=> routef "/user/%O" (fun id ->
        match Database.getUser id with
        | Ok user -> json user
        | Error _ -> setStatusCode 500
        )

    POST >=> route "/user" >=> bindJson<Database.UserData> (fun body ->
        match Database.addUser body with
        | Ok id -> text (string id)
        | Error _ -> setStatusCode 500
        )

    POST >=> routef "/user/%O" (fun id -> bindJson<Database.UserData> (fun body ->
        match Database.updateUser id body with
        | Ok 1 -> setStatusCode 200
        | _ -> setStatusCode 500
        ))

    setStatusCode 404 >=> text "not found, my friend"
  ]

// routef:
//
// Char  Type
// ----  ----
// %b    bool
// %c    char
// %s    string
// %i    int
// %d    int64
// %f    float/double
// %O    Guid (including short GUIDs*)
// %u    uint64 (formatted as a short ID*)
//

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
  with ex ->
    printfn "%A" ex
    1
