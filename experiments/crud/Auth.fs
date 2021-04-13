module Auth

open System
open FSharp.Control.Tasks
open Giraffe
open Microsoft.AspNetCore.Http


type Claims = {
  UserId: Guid;
  Email: string;
  }

let verifyUserToken (token : string) : Result<Guid, string> =
  Ok <| Guid.Parse "00000000-0000-0000-0000-000000000000"

let ensureClaimsNaive (next : HttpFunc) (ctx : HttpContext) =
  // Ensure the user token is valid, effectively verifying the claims within
  // This is fine, but additionally returning the claims would be
  // better. It would force/nudge us to use this function whenever we want to
  // extract the claim data.
  task {
    match Result.bind verifyUserToken (ctx.GetRequestHeader "Authorization") with
    | Ok guid ->
      return! next ctx
    | Error err ->
      // TODO: log the error
      return! setStatusCode 401 earlyReturn ctx
  }

let ensureClaims
  // Ensure the user token is valid, which verifies the claims and passes it to
  // another handler function. This way we can have one wrapper for passing claims
  // and can write our business logic without worrying about the authentication.
  (innerHandler : Claims -> HttpHandler)
  (next : HttpFunc)
  (ctx : HttpContext)
  : HttpFuncResult
  =
  task {
    match Result.bind verifyUserToken (ctx.GetRequestHeader "Authorization") with
    | Ok guid ->
      let claims = {UserId = guid; Email = ""}
      return! innerHandler claims earlyReturn ctx
    | Error err ->
      // TODO: log the error
      return! setStatusCode 401 earlyReturn ctx
  }
