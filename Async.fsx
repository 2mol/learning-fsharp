open System.Threading

let workThenWait () = async {
  printfn "let's go"
  Thread.Sleep(1000)
  printfn "work done"
  do! Async.Sleep(1000)
}

let work = workThenWait () |> Async.StartAsTask
printfn "started"
work.Wait()
printfn "completed"
