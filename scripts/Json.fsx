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

printfn "%s" <| JsonSerializer.Serialize(question)

let str = """{"id":"de368d97-903c-4460-85a3-0b27d8fef453","question_text":"what isn't","correct_answer":"yes","incorrect_answers":["a","b","c"]}"""

printfn "%A" <| JsonSerializer.Deserialize(str)
