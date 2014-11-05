namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("ScriptCs.FSharp")>]
[<assembly: AssemblyProductAttribute("ScriptCs.FSharp")>]
[<assembly: AssemblyDescriptionAttribute("A ScriptCs script engine for F#")>]
[<assembly: AssemblyVersionAttribute("0.2.0")>]
[<assembly: AssemblyFileVersionAttribute("0.2.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.2.0"
