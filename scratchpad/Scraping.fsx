open System.Globalization
#r "nuget: FSharp.Data"

// open FSharp.Charting

open FSharp.Data
open FSharp.Data.Runtime
open FSharp.Data.HtmlDocument
open FSharp.Data.HtmlNode

// type HtmlPage =
//   HtmlProvider<"https://apps.apple.com/ch/app/avec/id1454000074">

let simpleHtml = """<html>
                     <head>
                        <script language="JavaScript" src="/bwx_generic.js"></script>
                        <link rel="stylesheet" type="text/css" href="/bwx_style.css">
                        </head>
                    <body>
                        <img src="myimg.jpg">
                        <table title="table">
                            <tr><th>Column 1</th><th>Column 2</th></tr>
                            <tr><td>1</td><td>yes</td></tr>
                        </table>
                    </body>
                </html>"""

let result : HtmlDocument = HtmlDocument.Parse simpleHtml

// let bla =
//     let parameters : HtmlInference.Parameters =
//         { MissingValues = TextConversions.DefaultMissingValues
//           CultureInfo = CultureInfo.InvariantCulture
//           UnitsOfMeasureProvider = StructuralInference.defaultUnitsOfMeasureProvider
//           PreferOptionals = false }
//     HtmlRuntime.getTables (Some parameters) false result

printfn "%A" result
