﻿module LinkGeneratorTest

open Persimmon
open Persimmon.Syntax.UseTestNameByReflection
open Persimmon.MuscleAssert
open FSharpApiSearch
open TestAssemblies

let fsharpTest = parameterize {
  source [
    "Microsoft.FSharp.Core.Operators.( >= )", "operators.%5b-%5d%3d-%5d%5b%27t%5d-function-%5bfsharp%5d"
    "Microsoft.FSharp.Core.Operators.( |Failure|_| )", "operators.failure-active-pattern-%5bfsharp%5d"
    "Microsoft.FSharp.Core.Operators.incr", "operators.incr-function-%5bfsharp%5d"
    "Microsoft.FSharp.Control.Async.Start", "async.start-method-%5bfsharp%5d"
    "Microsoft.FSharp.Control.Async.CancellationToken", "async.cancellationtoken-property-%5bfsharp%5d"
    "Microsoft.FSharp.Collections.Map<'Key, 'Value>.new", "collections.map%5b%27key%2c%27value%5d-constructor-%5bfsharp%5d"
  ]
  run (fun (name, expected) -> test {
    let! apiDict = fscoreApi
    let api = apiDict.Api |> Array.find (fun a -> a.Name.Print() = name)
    let actual = LinkGenerator.FSharp.generate api
    do! actual |> assertEquals (Some expected)
  })
}

let msdnTest = parameterize {
  source [
    "System.Random", None
    "System.Random.Next", Some "system.random.next.aspx"
    "System.Random.NextBytes", Some "system.random.nextbytes.aspx"
    "System.Lazy<'T>.Value", None
  ]
  run (fun (name, expected) -> test {
    let! apiDict = mscorlibApi
    let api = apiDict.Api |> Array.find (fun a -> a.Name.Print() = name)
    let actual = LinkGenerator.Msdn.generate api
    do! actual |> assertEquals expected
  })
}