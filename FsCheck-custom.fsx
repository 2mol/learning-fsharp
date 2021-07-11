#r "nuget: FsCheck, 3.0.0-alpha5"

open FsCheck

type ThreeGenerator =
  static member ThreeMultiple() =
    Arb.generate<NonNegativeInt>
    |> Gen.map (fun (NonNegativeInt n) -> 3 * n)
    |> Gen.filter(fun n->  n> 0)
    |> Arb.fromGen
