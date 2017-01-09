module adwho

open adwho.core
open adwho.core.SearchResult.Getters
open Argu
open System
open System.DirectoryServices

[<CliPrefix(CliPrefix.DoubleDash)>]
[<NoAppSettings>]
type LsArguments =
    | [<Mandatory; MainCommand>] Search of search: string*string
    | [<AltCommandLine("-p")>] Prop of prop: string
    | [<AltCommandLine("-m")>] Multiple
    | [<AltCommandLine("-r")>] SearchRoot of root: string
with
    interface IArgParserTemplate with
        member arg.Usage =
            match arg with
            | Search _ -> "Search based on key value"
            | Prop _ -> "Optional property to get once a search result is returned. If omitted, program write the search result in JSON format"
            | Multiple -> "Returns multiple results in JSON format"
            | SearchRoot _ -> "Search root, example: LDAP://OU=UserAccounts,DC=foo,DC=bar,DC=baidu,DC=com"

let EXIT_NOTFOUND = -1

[<EntryPoint>]
let main argv =
    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<LsArguments>(errorHandler = errorHandler)
    let opts = parser.ParseCommandLine argv

    let de = 
        getRootDirectoryEntry ()
        |> match opts.TryGetResult <@ SearchRoot @> with
           | Some root -> scope root
           | _ -> id

    let searcher = de |> adwho.core.Searcher.fromDirectoryEntry
    let k, v = opts.GetResult(<@ Search @>)
    let filter = Filter.ofKeyValue k v

    if opts.Contains(<@ Multiple@>) then
        searcher filter
        |> Seq.map SearchResult.toDict
        |> SearchResult._formatJsonIndented
        |> printfn "%s"

        exit 0

    let result = searcher filter |> Seq.tryHead
    if result |> Option.isNone then
        printfn "No results found."
        exit EXIT_NOTFOUND

    let result = result.Value
    match opts.TryGetResult(<@ Prop @>) with
    | Some(prop) -> 
        match tryGetSingleKnownOrRaw prop result with
        | None -> printfn "Prop could not be found: %s" prop
        | Some(x) -> 
            x
            |> SearchResult._formatJsonIndented
            |> printfn "%s"
    | None -> 
        SearchResult.jsonify result |> printfn "%s"

    0 // return an integer exit code
