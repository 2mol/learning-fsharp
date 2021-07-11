#r "nuget: FsCheck"
#r "nuget: Expecto"
#r "nuget: Expecto.FsCheck"

open FsCheck
open Expecto
open Expecto.ExpectoFsCheck

let config = { FsCheckConfig.defaultConfig with maxTest = 10000 }

// let multipleOfThree n = n * 3

// let gen3 =
//   Arb.generate<NonNegativeInt>
//   |> Gen.map (fun (NonNegativeInt n) -> multipleOfThree n)
//   |> Gen.filter (fun n -> n > 5)
//   |> Arb.fromGen

let properties =
  testList "FsCheck samples" [
    testProperty "Addition is commutative" <| fun a b ->
      a + b = b + b

    testProperty "Reverse of reverse of a list is the original list" <|
      fun (xs:list<int>) -> List.rev (List.rev xs) = xs

    // you can also override the FsCheck config
    testPropertyWithConfig config "Product is distributive over addition" <|
      fun a b c ->
        a * (b + c) = a * b + a * c
  ]

Tests.runTests defaultConfig properties
