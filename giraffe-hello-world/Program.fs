open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http

let webApp : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        GET >=> choose [
            route "/" >=> text "index"
        ]
        setStatusCode 404 >=> text "Not Found"
    ]

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffe webApp

// let configureServices (services : IServiceCollection) =
//     services.AddGiraffe() |> ignore

[<EntryPoint>]
let main args =
    try
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                fun webHost ->
                    webHost
                        .Configure(configureApp)
                        // .ConfigureServices(configureServices)
                        |> ignore
                )
            .Build()
            .Run()
        0
    with ex ->
        1
