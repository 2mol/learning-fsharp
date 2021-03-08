module Database

open System
open System.Collections.Generic
open Npgsql.FSharp
open System.Text.Json.Serialization

// [<JsonFSharpConverter>]
// type Example = { x: string; y: string list }

type Gender =
  Unicorn | Guns | Socialism

type User = {
    UserId : Guid
    MandatorId : string
    Email : string option
    Gender : Gender
    // Birthdate: Date
    // Link: string option
}

let databaseName = "crud"

let connection =
    Sql.host "localhost"
    |> Sql.port 5432
    |> Sql.username "postgres"
    |> Sql.database databaseName
    |> Sql.config "Pooling=true" // optional Config for connection string

let initDb =
  connection
  |> Sql.database "postgres"
  |> Sql.connectFromConfig
  |> Sql.query $"CREATE DATABASE {databaseName}"
  |> Sql.executeNonQuery

let health =
  connection
  |> Sql.connectFromConfig
  |> Sql.query "UPDATE users SET meta = @meta"
  |> Sql.parameters [ "@meta", Sql.text "bla" ]
  |> Sql.executeNonQuery

let updateUserGeneric (attributes : IDictionary<string, string>) : Result<int, exn> =
  Ok 1


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
