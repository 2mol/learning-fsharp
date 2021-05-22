#r "nuget: FSharp.Data"

open FSharp.Data

type Pokemon = JsonProvider<"https://pokeapi.co/api/v2/pokemon/1">

let bulbasaur = Pokemon.GetSample()
