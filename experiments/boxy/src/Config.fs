module Config

open System
open System.IO
open FsConfig


type RuntimeEnvironment =
    | Production
    | Development

type Config =
    { RuntimeEnvironment: RuntimeEnvironment
      DummyVariable: string
      BummyVariable: string }

// let dotEnvFile : string = ".env"

// let setEnv variable value =
//     Environment.SetEnvironmentVariable (variable, value)

let setVariablesFrom (dotEnvFile: string): unit =
    let projectDirectory =
        AppDomain.CurrentDomain.BaseDirectory
        |> fun dir -> Path.Combine(dir, "../../..")
        |> Path.GetFullPath

    try
        Path.Combine(projectDirectory, dotEnvFile)
        |> File.ReadLines
        |> Seq.iter
            (fun line ->
                if line.StartsWith "#" then
                    () // skip comments
                else
                    match line.Split "=" |> Array.toList with
                    | [ variable; value ] ->
                        printfn "setting %s to %s" variable value
                        Environment.SetEnvironmentVariable(variable, value)
                    | _ -> ())
    with :? FileNotFoundException ->
        printfn
            "NOTE: no %s file found in %s. Will use existing environment variables instead"
            dotEnvFile
            projectDirectory
