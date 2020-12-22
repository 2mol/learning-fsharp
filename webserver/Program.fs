module Library

open System
open Newtonsoft.Json

let getJsonNetJson value =
  sprintf
    "I used to be %s but now I'm %s thanks to JSON.NET!"
    value (JsonConvert.SerializeObject(value))


[<EntryPoint>]
let main argv =
  printfn "Nice command-line arguments! Here's what JSON.NET has to say about them:"
  argv
  |> Array.map (getJsonNetJson >> printfn "%s")
  |> ignore
//   |> Array.iter (printfn "%s")
  0 // Return an integer exit code
