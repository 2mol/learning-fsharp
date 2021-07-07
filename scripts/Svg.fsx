#r "nuget: SharpVG"

open SharpVG

let position = Point.ofInts (10, 10)
let area = Area.ofInts (50, 50)
let style = Style.create (Color.ofName Colors.Cyan) (Color.ofName Colors.Blue) (Length.ofInt 3) 1.0 1.0

Rect.create position area
  |> Element.createWithStyle style
  |> printf "%O"
