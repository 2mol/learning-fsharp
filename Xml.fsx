open System
#r "nuget: FSharp.Data"

open FSharp.Data


let payloadRaw = """
<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<DeviceRequest WorkstationID="POS" RequestID="8.8" RequestType="Output" SequenceID="0" xmlns="http://www.nrf-arts.org/IXRetail/namespace">
    <Output OutDeviceTarget="E-Journal">
        <E-Journal>
            <ShopInfo>
                <ShopLocation>Valora Digital AG</ShopLocation>
                <ShopLocation>Hardturmstrasse 161</ShopLocation>
                <ShopLocation>8005 Z??rich</ShopLocation>
                <TerminalIdentifier>10133464</TerminalIdentifier>
                <MerchantUserIdentifier>JERTEST00000700</MerchantUserIdentifier>
            </ShopInfo>
            <TransactionInfo>
                <TransactionIdentifier>9142</TransactionIdentifier>
                <ServiceLabelName>PURCHASE</ServiceLabelName>
                <DateAndTime>2021-10-07T16:08:13</DateAndTime>
                <AuthorisationCode>103128</AuthorisationCode>
                <DetailedAmount>3.90</DetailedAmount>
                <TotalAmount Currency="CHF">3.90</TotalAmount>
                <TransactionResultText>00</TransactionResultText>
                <AcquirerIdentifier>2</AcquirerIdentifier>
                <ActivationSequenceNumber>6</ActivationSequenceNumber>
                <TransactionRefNumber>71907629331</TransactionRefNumber>
            </TransactionInfo>
            <CardInfo>
                <CardLabelName>MC CREDIT</CardLabelName>
                <ApplicationIdentifier>A0000000041010</ApplicationIdentifier>
                <CardNumber>XXXXXXXXXXXX0036</CardNumber>
                <AppPANEnc>02F4B6B056E58D8174250CAB49D85108</AppPANEnc>
                <AcqPubKeyInd>2</AcqPubKeyInd>
            </CardInfo>
            <DCCInfo/>
        </E-Journal>
    </Output>
</DeviceRequest>
<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<CardServiceResponse OverallResult="Success" WorkstationID="POS" RequestID="8" RequestType="CardPayment" xmlns="http://www.nrf-arts.org/IXRetail/namespace">
    <Terminal TerminalID="10133464" STAN="9142"/>
    <Tender LanguageCode="en">
        <TotalAmount Currency="CHF">3.90</TotalAmount>
        <Authorisation TimeStamp="2021-10-07T16:08:34.000+02:00" ApprovalCode="103128" TrxReferenceNumber="71907629331" CardCircuit="ECMC" MaskedCardNumber="xxxxxxxxxxxx0036" ReceiptCopies="1"/>
    </Tender>
    <PrivateData>
        <TokenItem TokenPurpose="Tracking" TokenResultCode="0" SecretID="Default" Token="999274559089263603"/>
    </PrivateData>
</CardServiceResponse>
"""

[<Literal>]
let xml_1 = """
<DeviceRequest WorkstationID="POS" RequestID="8.8" RequestType="Output" SequenceID="0" xmlns="http://www.nrf-arts.org/IXRetail/namespace">
    <Output OutDeviceTarget="E-Journal">
        <E-Journal>
            <ShopInfo>
                <ShopLocation>Valora Digital AG</ShopLocation>
                <ShopLocation>Hardturmstrasse 161</ShopLocation>
                <ShopLocation>8005 Z??rich</ShopLocation>
                <TerminalIdentifier>10133464</TerminalIdentifier>
                <MerchantUserIdentifier>JERTEST00000700</MerchantUserIdentifier>
            </ShopInfo>
            <TransactionInfo>
                <TransactionIdentifier>9142</TransactionIdentifier>
                <ServiceLabelName>PURCHASE</ServiceLabelName>
                <DateAndTime>2021-10-07T16:08:13</DateAndTime>
                <AuthorisationCode>103128</AuthorisationCode>
                <DetailedAmount>3.90</DetailedAmount>
                <TotalAmount Currency="CHF">3.90</TotalAmount>
                <TransactionResultText>00</TransactionResultText>
                <AcquirerIdentifier>2</AcquirerIdentifier>
                <ActivationSequenceNumber>6</ActivationSequenceNumber>
                <TransactionRefNumber>71907629331</TransactionRefNumber>
            </TransactionInfo>
            <CardInfo>
                <CardLabelName>MC CREDIT</CardLabelName>
                <ApplicationIdentifier>A0000000041010</ApplicationIdentifier>
                <CardNumber>XXXXXXXXXXXX0036</CardNumber>
                <AppPANEnc>02F4B6B056E58D8174250CAB49D85108</AppPANEnc>
                <AcqPubKeyInd>2</AcqPubKeyInd>
            </CardInfo>
            <DCCInfo/>
        </E-Journal>
    </Output>
</DeviceRequest>
"""

// type DataThing = XmlProvider<xml_1>
// let sample = DataThing.Parse(xml_1)

// printfn "%A" sample.WorkstationId


[<Literal>]
let PayloadSeparator =
    """<?xml version="1.0" encoding="utf-8" standalone="yes"?>"""

let bla =
    payloadRaw.Split([| PayloadSeparator |], StringSplitOptions.None)
    |> Array.map (fun str -> str.Trim('\r', '\n', ' '))
    |> Array.filter (fun str -> str.Length <> 0)

printfn "%A" bla
