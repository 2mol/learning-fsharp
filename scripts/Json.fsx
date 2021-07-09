open System
open System.Text.Json
open System.Text.Json.Serialization


type Question = {
  id : Guid
  [<JsonPropertyName("question_text")>]
  QuestionText : string
  [<JsonPropertyName("correct_answer")>]
  correctAnswer : string
  [<JsonPropertyName("incorrect_answers")>]
  incorrectAnswers : string array
}

let question = {
  id = Guid.NewGuid()
  QuestionText = "what is"
  correctAnswer = "no"
  incorrectAnswers = [| "a"; "b"; "c" |]
}

printfn "%A" <| JsonSerializer.Serialize(question)
