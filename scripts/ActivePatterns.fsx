type ProductPricing =
    | WeightPriced of int
    | UnitPriced

type Barcode =
    { ProductPricing : ProductPricing
      ProductIdentifier : string
      Checksum : char
    }

let (|ValidBarcode|_|) (barcodeStr : string) : Barcode option =
    try
        match barcodeStr.[0..1] with
        | "21"
        | "22" ->
            { ProductPricing = WeightPriced (int barcodeStr.[7..11])
              ProductIdentifier = barcodeStr.[0..6] + "000000"
              Checksum = barcodeStr.[12]
            }
        | _ ->
            { ProductPricing = UnitPriced
              ProductIdentifier = barcodeStr.[0..11]
              Checksum = barcodeStr.[12]
            }
        |> Some
    with
    | _ -> None

let parseBarcode codeStr =
    match codeStr with
    | ValidBarcode data ->
        Ok data
    | ex ->
        Error "Could not parse the checkin code"

let parseMePlease codeStr =
    match codeStr with
    | ValidBarcode data ->
        printfn "%A" data
    | _ ->
        printfn "no dice"

parseMePlease "2267721007055"
parseMePlease "7640143482672"
