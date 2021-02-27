open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

/// The messages dispatched by the view
type Msg =
    | Pressed

/// The model from which the view is generated
type Model =
    { Pressed: bool }

/// Returns the initial state
let init() = { Pressed = false }

/// The function to update the view
let update (msg: Msg) (model: Model) =
    match msg with
    | Pressed -> { model with Pressed = true }

/// The view function giving updated content for the page
let view (model: Model) dispatch =
    View.ContentPage(
        content=View.StackLayout(
            children=[
                if model.Pressed then
                    yield View.Label(text="I was pressed!")
                else
                    yield View.Button(text="Press Me!", command=(fun () -> dispatch Pressed))
            ]
        )
    )

type App () as app =
    inherit Application ()
    let runner =
        Program.mkSimple init update view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app
