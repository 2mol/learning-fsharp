[<Measure>] type m
[<Measure>] type kg
[<Measure>] type bmi = kg/m^2

let calcBmi (height : float<m>) (weight :float<kg>) : float<bmi>=
    weight / (height * height)

printfn "%A" <| calcBmi 1.79<m> 77.<kg>
