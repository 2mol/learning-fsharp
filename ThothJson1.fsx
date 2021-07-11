#r "nuget: Thoth.Json.Net"

open Thoth.Json.Net

Decode.fromString Decode.string "\"maxime\""

type Point =
  { X : int
    Y : int
    Z : int
  }

let decoder : Decoder<Point> =
  Decode.object (fun get ->
    { X = get.Required.Field "x" Decode.int
      Y = get.Required.Field "y" Decode.int
      Z = get.Required.Field "z" Decode.int
    }
  )

printfn "%A" <| Decode.fromString decoder """{"x": 10, "y": 21}"""
printfn "%A" <| Decode.fromString decoder """{"x": 10, "y": 21, "z": 666}"""
