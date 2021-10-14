#r "nuget: Npgsql"
#r "nuget: Dapper.FSharp"

open System
open System.Data

open Npgsql
open Dapper.FSharp
open Dapper.FSharp.PostgreSQL

Dapper.FSharp.OptionTypes.register()

let connectionString = "Host=localhost; Database=juri; Username=juri;"
let conn : IDbConnection = new NpgsqlConnection(connectionString) :> IDbConnection

type Person = {
    id : Guid
    first_name : string
    last_name : string
    position : int
    date_of_birth : DateTime option
}

let personTable = table'<Person> "person"


insert {
    into personTable
    value { id = Guid.NewGuid(); first_name = "Roman"; last_name = "Provaznik"; position = 1; date_of_birth = Some DateTime.Today }
}
|> conn.InsertAsync
|> Async.AwaitTask
|> Async.RunSynchronously


let bla =
    select {
        for p in personTable do
        selectAll
    }
    |> conn.SelectAsync<Person>
    |> Async.AwaitTask
    |> Async.RunSynchronously

printfn "%A" bla
