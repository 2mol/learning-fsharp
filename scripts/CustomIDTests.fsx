#r "nuget: FsCheck"
#r "nuget: Expecto"
#r "nuget: Expecto.FsCheck"

open System

open FsCheck
open Expecto
open Expecto.ExpectoFsCheck


let private ensureCorrectByteOrder arr =
    if BitConverter.IsLittleEndian then arr |> Array.rev else arr

let getEncodedTimestamp (getCurrentTimestamp: unit -> int64) () =
    getCurrentTimestamp()
    |> BitConverter.GetBytes
    |> ensureCorrectByteOrder
    |> Array.truncate 8 // we limit the numbers to being 8 bytes long, it's enough for timestamps
    |> Convert.ToBase64String
    |> fun s -> s.Substring(5,6) // first 5 characters are "A" we descard them, the last character is "=", also uninteresting for us

let decodeTimestamp (encoded: string) =
    $"AAAAA{encoded}="
    |> Convert.FromBase64String
    |> ensureCorrectByteOrder
    |> fun arr -> BitConverter.ToInt64(arr, 0)

let getEncodedCurrentTimestamp () = getEncodedTimestamp DateTimeOffset.UtcNow.ToUnixTimeSeconds ()

/// Checkin
/// Reference numbers have to be up to 20 characters long. We want to keep encodeing the store id of the checkin but additionally
/// guarantee uniqueness of the reference number to a certain degree
/// We can achieve this with a following format new format:
/// 12345-123456-123 <- 16 characters, composed as follows:
/// storeId; "-" separator; encoded timestamp; first 3 characters of user's id
/// Using this approach effectively limits customer to 1 checkin per second, but
/// in contrast to an approach where we would generate somthing random, past references are guaranteed not to interfere
/// with future ones. (plus one checkin per second seems like a reasonable limitation)
let getCheckinReferenceNumberPure getCurrentTimestamp (storeId: string) (userId: Guid) =
    let encodedTimestamp = getEncodedTimestamp getCurrentTimestamp ()
    let userIdSuffix = userId.ToString().Substring(0, 3)
    $"{storeId}-{encodedTimestamp}-{userIdSuffix}"

let getCheckinReferenceNumber (storeId: string) (userId: Guid) =
    getCheckinReferenceNumberPure DateTimeOffset.UtcNow.ToUnixTimeSeconds storeId userId

/// Checkout
/// On checkout we use cartId to guarantee uniqueness, however in an edge case where we need to cancel someone's transaction
/// to retry it later, paypal treats this id as no longer unique
/// The solution is to add 1 random character at the end of the reference number.
/// This will the probability of collision will increase with the number of retries according to formula
/// p = 1 - 62! / ((62-n)! * 62^n)
/// The reference number will end up looking as follows:
/// 12345-EK1234567890-a
let getCheckoutReferenceNumberPure getRandomCharacter (storeId: string) (cartId: string) =
    let randomCharacter = getRandomCharacter()
    $"{storeId}-EK{cartId}-{randomCharacter}"

/// Random in .net has a pretty poor API. The `Random` object should be only instantiated when absolutely necessary
/// so one should reuse the instance, but on the other hand it is not thread-safe so we have to lock it.
/// Cool article series about it: https://ericlippert.com/2019/01/31/fixing-random-part-1/
let private rnd = new Random()
let private lockObj = new Object()
let [<Literal>] Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
let private getRandomCharacter () =
    lock lockObj (fun _ ->
        Alphabet.[rnd.Next(62)]
    )

let getCheckoutReferenceNumber (storeId: string) (cartId: string) =
    getCheckoutReferenceNumberPure getRandomCharacter storeId cartId



type TimestampGenerator =
  static member Timestamp () =
      Arb.generate<int64>
      |> Gen.filter (fun n -> n >= 0L)
    //   |> Gen.map(fun n -> fun (_ : unit) -> n)
      |> Arb.fromGen
    //   |> Arb.filter (fun n -> n >= 0L)


let config = {
  FsCheckConfig.defaultConfig
    with maxTest = 10000; arbitrary = [typeof<TimestampGenerator>]
  }

let properties =
  testList "test all" [
    testPropertyWithConfig config  "things are decodable" <| fun (timeStamp : int64) ->
      // let timeStamp = timeStampgen ()
      let str = getEncodedTimestamp (fun _ -> timeStamp) ()
      // if (decodeTimestamp str <> timeStamp) then
      printf "wrong with %A" str
      printf $"wrong with ts {timeStamp}"
      decodeTimestamp str = timeStamp
  ]

Tests.runTests defaultConfig properties
