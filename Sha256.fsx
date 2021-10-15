open System.Security.Cryptography
open System.Text

let sha = SHA256.Create()

let thing =
    "6149ebcc74bce094234eb29d"
    |> Encoding.ASCII.GetBytes
    |> sha.ComputeHash
    |> Seq.map (fun b -> b.ToString("x2"))
    |> String.concat ""

printfn "%A" thing
