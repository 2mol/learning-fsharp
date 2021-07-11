#r "nuget: Thoth.Json.Net"

open Thoth.Json.Net

type Address =
  { Street : string
    Number : int16
    PostCode : int16
    City : string
  }

  static member Decoder : Decoder<Address> =
    Decode.object (fun get ->
      { Street = get.Required.Field "street" Decode.string
        Number = get.Required.Field "number" Decode.int16
        PostCode = get.Required.Field "postcode" Decode.int16
        City = get.Required.Field "city" Decode.string
      }
    )

type User =
  { Id : int
    Name : string
    Email : string
    Address : Address option
  }

  static member Decoder : Decoder<User> =
    Decode.object (fun get ->
      { Id = get.Required.Field "id" Decode.int
        Name = get.Required.Field "name" Decode.string
        Email = get.Required.Field "email" Decode.string
        Address = get.Optional.Field "address" Address.Decoder
      }
    )

printfn "%A" <| Decode.fromString User.Decoder """
  { "id": 666
  , "name": "Juri Chomé"
  , "email": "juri@fastmail.com"
  , "address":
    { "street": "Manessestrasse"
    , "number": 132
    , "postcode": 8045
    , "city": "Zürich"
    }
  }
  """
