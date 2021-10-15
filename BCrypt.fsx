#r "nuget: BCrypt.Net-Next"

open BCrypt.Net

let thing =
    "6149ebcc74bce094234eb29d"
    |> BCrypt.HashPassword

// let bytes = Encoding.ASCII.GetBytes(value)

printfn "%A" thing
