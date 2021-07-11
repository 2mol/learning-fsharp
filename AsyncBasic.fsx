open System.Threading

let workThenWait () = async {
  Thread.Sleep(1000)
  printfn "work done"
  do! Async.Sleep(1000)
}

let work = workThenWait () |> Async.StartAsTask
printfn "started"
work.Wait()
printfn "completed"
