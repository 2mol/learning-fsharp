#r "nuget: Npgsql.FSharp"
#r "nuget: FSharp.SystemTextJson"
#r "nuget: Thoth.Json.Net"

open System
open Npgsql.FSharp
open System.Text.Json
open System.Text.Json.Serialization


let jsonOptions = JsonSerializerOptions()
jsonOptions.Converters.Add(JsonFSharpConverter())

let deserialize (str : string) =
    JsonSerializer.Deserialize(str, jsonOptions)

let connection =
    Sql.host "localhost"
    |> Sql.port 5432
    |> Sql.username "postgres"
    |> Sql.database "dojo"

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

let user =
    connection
    |> Sql.connectFromConfig
    |> Sql.query "SELECT * FROM users where id = @userid"
    |> Sql.parameters [ "@userid", Sql.uuid (Guid.Parse "000e1747-b664-47df-9ed9-35133ba3d039") ]
    |> Sql.executeRow (fun read ->
    {
        Id = read.uuid "id"
        Data = read.text "data" |> deserialize
        CreatedAt = read.dateTime "created_at"
    })

printfn "user: %A" user



// let someUserData = {
//   name = "John"
//   email = None
//   email_verified = false
//   birthdate = DateTime.ParseExact("31-01-2011", "dd-MM-yyyy", Globalization.CultureInfo.CurrentCulture)
//   favourite_number = Some 666
// }

// let udStr : string = JsonSerializer.Serialize(someUserData, jsonOptions)
// let udTest = JsonSerializer.Deserialize<UserData>(udStr, jsonOptions)

// printfn "user: %A" udTest
