#r "nuget: Ply"

// from Michal's Friday dojo

open System.Threading.Tasks
open FSharp.Control.Tasks
open System

type SimpleTimer(name) =
    let timer = Diagnostics.Stopwatch.StartNew()
    interface IDisposable with
        member _.Dispose() =
            timer.Stop()
            printfn $"{name} elapsed {timer.ElapsedMilliseconds}ms"


// raw Tasks vs computation expression
let rawTasks () =
    Task.Delay(500)
        .ContinueWith(fun _ ->
            Task.Delay(2000)
                .ContinueWith(fun _ ->
                    Task.FromResult("wut")
                        .ContinueWith(fun (t:Task<string>) -> "lol" + t.Result))).Unwrap().Unwrap()


let taskBuilder () =
    task {
        do! Task.Delay 500
        do! Task.Delay 300
        let! t = Task.FromResult "wut"
        return "lol" + t
    }


// Tasks
let task1 (delayMs: int) =
    task {
        printfn "starting task1"
        do! Task.Delay delayMs
        printfn "done with task1"
        return 42
    }

let task2 (delayMs: int) =
    task {
        printfn "starting task2"
        do! Task.Delay delayMs
        printfn "done with task2"
        return 27
    }

let task3 () =
    task {
        printfn "starting task3"
        let! result1 = task1 500
        let! result2 = task2 500
        return result1 + result2
    }

let task3hotStart () =
    task {
        printfn "starting task3"
        let result1Task = task1 500
        let result2Task = task2 500
        let! result1 = result1Task
        let! result2 = result2Task
        return result1 + result2

        // Alternatively we can wait on all tasks like so:
        // let! results = Task.WhenAll([task1 500; task2 500])
        // return results |> Array.sum
    }

let testTasks1 () = task3().GetAwaiter().GetResult()
let testTasks2 () = task3hotStart().GetAwaiter().GetResult()


// Async
let async1 (delayMs: int) =
    async {
        printfn "starting async1"
        do! Async.Sleep delayMs
        printfn "done with async1"
        return 42
    }

let async2 (delayMs: int) =
    async {
        printfn "starting async2"
        do! Async.Sleep delayMs
        printfn "done with async2"
        return 27
    }

let async3 () =
    async {
        printfn "starting async3"
        let asyncresult1 = async1 500
        let asyncresult2 = async2 500
        let! result1 = asyncresult1
        let! result2 = asyncresult2
        return result1 + result2
    }

let async3Parallel () =
    async {
        printfn "starting async3"
        let! results = [ async1 500; async2 500] |> Async.Parallel
        return results |> Array.sum
    }

let testAsync () = async3 () |> Async.RunSynchronously
let testAsyncParallel () = async3Parallel () |> Async.RunSynchronously



// yield points
let loopyLoop times =
    let rec inner state score =
        if score = times then
            ()
        else
            if state = Int32.MaxValue then
                inner 0 (score+1)
            else
                inner (state+1) score
    inner 0 0

let task4 name =
    task {
        // do! Task.Yield() // do this to yield control immediately
        printfn "my name is %s" name
        printfn "%s is gonna loop" name
        do loopyLoop 2
        printfn "%s is gonna rest" name
        do! Task.Delay 1000
        printfn "%s is gonna loop some more" name
        do loopyLoop 1
        printfn "%s is done looping" name
        return 42
    }

let task5 () =
    task {
        use _ = new SimpleTimer("task 5")
        let! first = task4 "first"
        let! second = task4 "second"
        return first + second
    }

let task5concurrent () =
    task {
        use _ = new SimpleTimer("task 5 concurrent")
        let firstTask = task4 "first"
        let secondTask = task4 "second"
        let! first = firstTask
        let! second = secondTask
        return first + second
        // Alternatively we can wait on all tasks like so:
        // let! results = Task.WhenAll([task4 "first"; task4 "second"])
        // return results |> Array.sum
    }

// we have to open Linq to use Parallel Linq extensions, Parallel.For(each) is super cumbersome to use
open System.Linq
let task5parallel () =
    task {
        use _ = new SimpleTimer("task 5 parallel")
        let firstTask () = task4 "first"
        let secondTask () = task4 "second"
        let runningTasks = [| firstTask; secondTask |].AsParallel().Select(fun t -> t())
        let! results = Task.WhenAll(runningTasks)
        return results |> Array.sum
    }


let async4 name =
    async {
        printfn "my name is %s" name
        printfn "%s is gonna loop" name
        do loopyLoop 2
        printfn "%s is gonna rest" name
        do! Async.Sleep 1000
        printfn "%s is gonna loop some more" name
        do loopyLoop 1
        printfn "%s is done looping" name
        return 42
    }

let async5 () =
    async {
        use _ = new SimpleTimer("task 5")
        let! first = async4 "first"
        let! second = async4 "second"
        return first + second
    } |> Async.RunSynchronously

let async5concurrent () =
    async {
        use _ = new SimpleTimer("task 5")
        let! results =
            [async4 "first"; async4 "second"]
            |> Async.Parallel
        return results |> Array.sum
    } |> Async.RunSynchronously

// rerunning workflows
let task6 () =
    task {
        let testTask = task4 "test"
        printfn "accessing first result"
        let! result = testTask
        printfn "accessing second result"
        let! rerun = testTask
        return result + rerun
    }

let async6 () =
    async {
        let testTask = async4 "test"
        printfn "accessing first result"
        let! result = testTask
        printfn "accessing second result"
        let! rerun = testTask
        return result + rerun
    } |> Async.RunSynchronously
