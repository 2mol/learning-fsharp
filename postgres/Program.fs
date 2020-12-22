// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open Npgsql.FSharp

let connection  =
    Sql.host "localhost"
    |> Sql.port 5432
    |> Sql.username "juri"
    |> Sql.database "readnotes"
    |> Sql.config "Pooling=true" // optional Config for connection string


type Thing = {
    Id: Guid
    Title: string
    Author: string option
    Year: int option
    Link: string option
}

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

[<EntryPoint>]
let main argv =
    let message = from "F#" // Call the function
    // printfn "Hello worldy %s" message
    let things =
        connection
        |> Sql.connectFromConfig
        |> Sql.query "SELECT * FROM things"
        |> Sql.execute (fun read ->
            {
                Id = read.uuid "id"
                Title = read.text "title"
                Author = read.textOrNone "author"
                Year = read.intOrNone "year"
                Link = read.textOrNone "link"
            })
    printfn "things: %A" things
    0 // return an integer exit code
