#if INTERACTIVE
#load "../load.fsx"
#endif

#load "adwholib.fs"
open adwho.core
open SearchResult.Getters

open System
open System.DirectoryServices

let k = "name"
let v = "John Doe"
let prop = "name"

let de = new DirectoryEntry()
let res = Searcher.fromDirectoryEntry de (Filter.ofKeyValue k v) |> Seq.head
res |> SearchResult.jsonify |> printfn "%s"

//getSingle prop res :?> Guid
getSingleKnownOrRaw "alias" res