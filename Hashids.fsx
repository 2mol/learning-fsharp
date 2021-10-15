#r "nuget: Hashids-fs"

open Hashids

let config =
    HashidConfiguration.create HashidConfiguration.defaultOptions

printfn "%A" <| Hashid.encode64 config [| 73L; 88L |]


// let thing =
//     "6149ebcc74bce094234eb29d"
//     |> Seq.map int64
//     |> Seq.toArray
//     |> Hashid.encode64 config

// printfn "%A" thing
