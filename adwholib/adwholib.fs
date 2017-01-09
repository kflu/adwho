namespace adwho

open System
open System.Collections
open System.DirectoryServices
open Newtonsoft.Json

module core =
    
    type searcher = string -> SearchResult seq

    let getRootDirectoryEntry _ = new DirectoryEntry()
    let scope path (de: DirectoryEntry) =
        de.Path <- path
        de

    module Searcher =
        let fromDirectoryEntry (de : DirectoryEntry) : searcher =
            fun filter ->
                let ds = new DirectorySearcher(de, Filter=filter)
                ds.FindAll() |> Seq.cast<SearchResult>
        
    module Filter =
        let ofKeyValue k v = sprintf "(%s=%s)" k v
    
    module SearchResult =
        let _formatJsonIndented x = JsonConvert.SerializeObject(x, Formatting.Indented)

        let toKeyValueSeq (res: SearchResult) =
            res.Properties |> Seq.cast<DictionaryEntry> |> Seq.map (fun x -> x.Key, x.Value)
        
        let toDict (res: SearchResult) = res.Properties :> IDictionary
        
        let jsonify res = res |> toDict |> _formatJsonIndented

        module Getters =
            type getter<'t> = SearchResult -> 't

            let getSingleRaw (name:string) (res: SearchResult) =
                res.Properties.[name].[0]
            
            let getSingleT<'t> name res = getSingleRaw name res :?> 't
            
            let getName : getter<_> = getSingleT<string> "name"

            let mutable KnownProps : (string * (SearchResult -> obj)) list = 
                [
                    "name", getName >> box
                ]
                |> List.map (fun (n, g) -> n.ToLower(), g) // name to lower case
            
            let setKnownProps xs = KnownProps <- xs |> List.map (fun (n:string, g) -> n.ToLower(), g)
            let addKnownProp (name: string, getter) =
                KnownProps <- (name.ToLower(), getter) :: KnownProps
            
            let (?=) x y = System.String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase)

            let getSingleKnownOrRaw name res = 
                match KnownProps |> Seq.tryFind (fun (n, g) -> n ?= name) with
                | None -> getSingleRaw name res
                | Some(n, g) -> g res
            
            let tryGetSingleKnownOrRaw name res =
                try getSingleKnownOrRaw name res |> Some
                with _ -> None