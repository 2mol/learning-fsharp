type AccountBalance =
    { Balance : decimal}

type BankAccount
    = Closed
    | Open of AccountBalance

type Msg
    = SetState of BankAccount
    | GetState of AsyncReplyChannel<BankAccount>
    | UpdateBalance of decimal

let mailboxProcessor (inbox : MailboxProcessor<Msg>) =
    let rec messageLoop (currentState : BankAccount) = async {
        let! msg = inbox.Receive ()
        match msg with
        | SetState newState -> return! messageLoop newState
        | GetState replyChannel ->
            replyChannel.Reply currentState
            return! messageLoop currentState
        | UpdateBalance amount ->
            match currentState with
            | Closed -> return! messageLoop Closed
            | Open {Balance=balance} ->
                let newState = Open { Balance = balance + amount }
                return! messageLoop newState
    }
    messageLoop

let mkBankAccount () =
    MailboxProcessor<Msg>.Start(
        fun inbox -> mailboxProcessor inbox Closed
    )

let openAccount (account : MailboxProcessor<Msg>) =
    Open { Balance = 0.0m}
    |> SetState
    |> account.Post
    account

let closeAccount (account : MailboxProcessor<Msg>) =
    Closed
    |> SetState
    |> account.Post
    account

let getBalance (account : MailboxProcessor<Msg>) =
    let acc = account.PostAndReply GetState
    match acc with
    | Closed -> None
    | Open { Balance = balance } -> Some balance

let updateBalance
    (amount : decimal)
    (account : MailboxProcessor<Msg>)
    =
    account.Post (UpdateBalance amount)
    account

// ----------------------------------------------------------------------------

let account =
    mkBankAccount()
    |> openAccount

let updateAccountAsync =
    async {
        account
        |> updateBalance 1.0m
        |> ignore
    }

updateAccountAsync
|> List.replicate 1000
|> Async.Parallel
|> Async.RunSynchronously
|> ignore

printfn "%A" <| getBalance account
