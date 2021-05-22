#r "nuget: FSharp.Data"

open FSharp.Data

type Pokemon = JsonProvider<"https://pokeapi.co/api/v2/pokemon/1">

let bulbasaur = Pokemon.GetSample()


let pokeApiRoot = "https://pokeapi.co/api/v2/pokemon"

let genOne = [ for index in 1..150 do Pokemon.Load($"{pokeApiRoot}/{index}") ]
    // There are better ways to get the original 150 but this works for our demo
    // https://pokeapi.co/api/v2/pokemon?limit=150

let tallestPokemonInGenOne =
    genOne
    |> List.sortByDescending (fun pokemon -> pokemon.Height)
    |> List.truncate 15
