open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors

let app =
  choose [
    GET >=> choose [
      path "/hello" >=> OK "Hello GET"
      path "/goodbye" >=> OK "Good bye GET"
      ]
    POST >=> choose [
      path "/hello" >=> OK "Hello POST"
      path "/goodbye" >=> OK "Good bye POST"
      ]
    NOT_FOUND "Resource not found."
  ]

// let config = { defaultConfig with homeFolder = Some "./wwwroot" }

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig app
    0
