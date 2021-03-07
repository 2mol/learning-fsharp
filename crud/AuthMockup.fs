module AuthMockup

open System
open FSharp.Control.Tasks
open Giraffe
open Microsoft.AspNetCore.Http


// let ensureAuthentication (token : string) : HttpHandler =
//   fun (next : HttpFunc) (ctx : HttpContext) ->
//     setStatusCode 401

let verifyUserToken (token : string) : Result<Guid, string> =
  Ok <| Guid.Parse "00000000-0000-0000-0000-000000000000"

let ensureUser (next : HttpFunc) (ctx : HttpContext) =
  // ensure the user token is valid, effectively verifying the claims within
  // this is good, but a function that would return the claims would be
  // better. It would force/nudge us to use this function whenever we want to
  // extract the claim data.
  task {
    match Result.map verifyUserToken (ctx.GetRequestHeader "Authentication") with
    | Ok guid ->
      // TODO: somehow return the data as well
      return! next ctx
    | Error err ->
      // TODO: log the error
      return! setStatusCode 401 earlyReturn ctx
  }
