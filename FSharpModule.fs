namespace ScriptCs.FSharp
open ScriptCs.Hosting
open ScriptCs.Contracts

[<Module("fsharp", Extensions="fs,fsx")>]
type FSharpModule () =
    interface IModule with
        member x.Initialize(builder) =
            builder.ScriptEngine<FSharpScriptEngine>()
            |>ignore
