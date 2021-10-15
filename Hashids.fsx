#r "nuget: Hashids-fs"

open Hashids

let config =
    HashidConfiguration.create HashidConfiguration.defaultOptions

printfn "%A" <| Hashid.encode64 config [| 73L; 88L |]
