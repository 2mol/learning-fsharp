module Database

open System
open Npgsql.FSharp
open System.Text.Json.Serialization

// [<JsonFSharpConverter>]
// type Example = { x: string; y: string list }

let databaseName = "crud"

let connection =
    Sql.host "localhost"
    |> Sql.port 5432
    |> Sql.username "postgres"
    |> Sql.database databaseName
    |> Sql.config "Pooling=true" // optional Config for connection string

type Thing = {
    Id: Guid
    Title: string
    Author: string option
    Year: int option
    Link: string option
}

let initDb =
  connection
  |> Sql.database "postgres"
  |> Sql.connectFromConfig
  |> Sql.query $"CREATE DATABASE {databaseName}"
  |> Sql.executeNonQuery

let health =
  connection
  |> Sql.connectFromConfig
  |> Sql.query "SELECT 1 as up"
  |> Sql.executeRow (fun read -> read.int "up")

// let doStuff =
//     let things =
//         connection
//         |> Sql.connectFromConfig
//         |> Sql.query "SELECT * FROM things"
//         |> Sql.execute (fun read ->
//             {
//                 Id = read.uuid "id"
//                 Title = read.text "title"
//                 Author = read.textOrNone "author"
//                 Year = read.intOrNone "year"
//                 Link = read.textOrNone "link"
//             })
//     printfn "things: %A" things
