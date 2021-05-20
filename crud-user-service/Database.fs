module Database

open System
open Npgsql.FSharp
open System.Text.Json
open System.Text.Json.Serialization

let databaseName = "userservice"

let jsonOptions = JsonSerializerOptions()
jsonOptions.Converters.Add(JsonFSharpConverter())

let serialize whatever = JsonSerializer.Serialize(whatever, jsonOptions)
let deserialize (str : string) =
    JsonSerializer.Deserialize(str, jsonOptions)

type UserData = {
    name : string
    email : string option
    email_verified : bool
    birthdate : DateTime // yes, I know, but there is no simple Date type.
    favourite_number : int option
}

type User = {
    Id : Guid
    Data : UserData
    CreatedAt : DateTime
}

let connection =
    Sql.host "localhost"
    |> Sql.port 5432
    |> Sql.username "postgres"
    |> Sql.database databaseName
    |> Sql.config "Pooling=true" // optional Config for connection string

let getUser id =
    try
        connection
        |> Sql.connectFromConfig
        |> Sql.query "SELECT * FROM users where id = @userid"
        |> Sql.parameters [ "@userid", Sql.uuid id ]
        |> Sql.executeRow (fun read ->
        {
          Id = read.uuid "id"
          Data = read.text "data" |> deserialize
          CreatedAt = read.dateTime "created_at"
        })
        |> Ok
    with
    | ex ->
        printfn "Database error: \n%A" ex.Message
        Error ()

let addUser userData =
    try
        connection
        |> Sql.connectFromConfig
        |> Sql.query "insert into users (data) values (@data) returning id"
        |> Sql.parameters [ "@data", Sql.jsonb <| serialize userData ]
        |> Sql.executeRow (fun read -> read.uuid "id")
        |> Ok
    with
    | ex ->
        printfn "Database error: \n%A" ex.Message
        Error ()

let updateUser id userData =
    try
        connection
        |> Sql.connectFromConfig
        |> Sql.query "update users set data = @data where id = @userid"
        |> Sql.parameters [ "@data", Sql.jsonb <| serialize userData; "@userid", Sql.uuid id ]
        |> Sql.executeNonQuery
        |> Ok
    with
    | ex ->
        printfn "Database error: \n%A" ex.Message
        Error ()
