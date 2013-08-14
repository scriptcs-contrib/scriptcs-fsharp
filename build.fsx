#if BOOT
open Fake
module FB = Fake.Boot
FB.Prepare {
    FB.Config.Default __SOURCE_DIRECTORY__ with
        NuGetDependencies =
            let (!!) x = FB.NuGetDependency.Create x
            [
                !!"FAKE"
                !!"NuGet.Build"
                !!"NuGet.Core"
            ]
}
#endif

#load ".build/boot.fsx"

open System.IO
open Fake 
open Fake.AssemblyInfoFile
open Fake.MSBuild

(* properties *)
let projectName = "ScriptCs.FSharp"
let version =
    if isLocalBuild then
        "0.1." + System.DateTime.UtcNow.ToString("yMMdd") + "-alpha"
    else buildVersion
let projectSummary = "A ScriptCs script engine for F#."
let projectDescription = "A ScriptCs script engine for F#."
let authors = ["7sharp9"; "gblock"; "panesofglass"]
let mail = "ryan.riley@panesofglass.org"
let homepage = "https://github.com/glennblock/scriptcs-fsharp"

(* Directories *)
let buildDir = "./build/"
let packagesDir = "./packages/"

let nugetDir = "./nuget/"
let nugetLibDir = nugetDir @@ "lib/net45"

(* Tools *)
let nugetPath = ".nuget/nuget.exe"

let RestorePackageParamF = 
  fun _ ->{ ToolPath = nugetPath
            Sources = []
            TimeOut = System.TimeSpan.FromMinutes 5.
            OutputPath = "./packages" 
           } :Fake.RestorePackageHelper.RestorePackageParams

let RestorePackages2() = 
  !! "./**/packages.config"
  |> Seq.iter (RestorePackage RestorePackageParamF)
RestorePackages2()

(* files *)
let appReferences =
    !+ "/**/*.fsproj" 
        |> Scan

(* Targets *)
Target "Clean" (fun _ ->
    CleanDirs [buildDir; nugetDir; nugetLibDir]
)

Target "BuildApp" (fun _ -> 
    if not isLocalBuild then
        [ Attribute.Version(version)
          Attribute.Title(projectName)
          Attribute.Description(projectDescription)
          Attribute.Guid("113CDD96-0824-491E-B332-62D4FDE02A1E")
        ]
        |> CreateFSharpAssemblyInfo "AssemblyInfo.fs"

    MSBuildRelease buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

Target "CopyLicense" (fun _ ->
    [ "LICENSE" ] |> CopyTo buildDir
)

Target "BuildNuGet" (fun _ ->
    [ buildDir + "ScriptCs.FSharp.dll"
      buildDir + "ScriptCs.FSharp.pdb" ]
        |> CopyTo nugetLibDir

    let fsiVersion = GetPackageVersion packagesDir "FsiEval"
    let scriptcsVersion = GetPackageVersion packagesDir "ScriptCs.Hosting"
    NuGet (fun p ->
        {p with
            Authors = authors
            Project = projectName
            Description = projectDescription
            Version = version
            OutputPath = nugetDir
            Dependencies = [ "FsiEval", fsiVersion
                             "ScriptCs.Hosting", scriptcsVersion ]
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            ToolPath = nugetPath
            Publish = hasBuildParam "nugetkey" })
        "ScriptCs.FSharp.nuspec"
)

Target "Deploy" DoNothing
Target "Default" DoNothing

(* Build Order *)
"Clean"
    ==> "BuildApp" <=> "CopyLicense"
    ==> "BuildNuGet"
    ==> "Deploy"

"Default" <== ["Deploy"]

// start build
RunTargetOrDefault "Default"

