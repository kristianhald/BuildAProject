#r @"packages/FAKE/tools/FakeLib.dll"
#r @"System.Xml.Linq"
open Fake
open System.Text.RegularExpressions
open System.Xml.XPath
open System.Xml.Linq

RestorePackages()

// Properties
let baseDir = "./compile/"
let buildDir = baseDir + "build/"
let testDir = baseDir + "test/"
let deployDir = baseDir + "deploy/"
let localFeedDir = "../LocalFeed/"

// Version info
let version = "0.1" // Should look at the assembly info and the patch version should be taken from the latest repository changeset

// Helper functions
let getNonTestProjects =
    !! "**/*.csproj"
        -- "**/*.Test*"
        -- "**/*.Test*/**"

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [buildDir; testDir; deployDir; baseDir]
)

Target "BuildApp" (fun _ -> 
    getNonTestProjects
        |> MSBuildRelease buildDir "Build"
        |> Log "AppBuild-Output: "
)

Target "BuildTest" (fun _ ->
    !! "**/*Test*.csproj"
        |> MSBuildDebug testDir "Build"
        |> Log "TestBuild-Output: "
)

Target "Test" (fun _ ->
    !! (testDir + "*.Test*.dll")
        |> NUnitParallel (fun p ->
            {p with
                DisableShadowCopy = true
                OutputFile = testDir + "TestResults.xml"})
)

let assemblyName projectPath = 
    let projectXml = XDocument.Load((string)projectPath)
    let xmlns = projectXml.Root.GetDefaultNamespace()
    let assemblyNameNodes = projectXml.Root.Descendants(xmlns.GetName "AssemblyName")
    (Seq.head assemblyNameNodes).Value
let frameworkVersion projectPath =
    let projectXml = XDocument.Load((string)projectPath)
    let xmlns = projectXml.Root.GetDefaultNamespace()
    let targetFrameworkVersionNodes = projectXml.Root.Descendants(xmlns.GetName "TargetFrameworkVersion")
    (Seq.head targetFrameworkVersionNodes).Value
let convertToNuGetFrameworkVersion frameworkVersion =
    "net" + Regex.Replace(frameworkVersion, "[^\d]", "")
let getLibraryPath assemblyName =
    buildDir + assemblyName + ".dll"
let getExecutablePath assemblyName =
    buildDir + assemblyName + ".exe"
let copyLibraryToPackagingDir libraryPath = 
    Copy deployDir libraryPath
let getNugetDependencies (packagesFile:string) =
    let xname = XName.op_Implicit
    let attribute name (e:XElement) =
        match e.Attribute (xname name) with
        | null -> ""
        | a -> a.Value
    let doc = 
        XDocument.Load packagesFile
    [for package in doc.Descendants (xname"package") ->
        attribute "id" package, attribute "version" package, attribute "targetFramework" package ]
Target "LocalPackage" (fun _ ->
    getNonTestProjects
        |> Seq.toArray
        |> Seq.map assemblyName
        |> Seq.map (fun assemblyName -> if fileExists (getLibraryPath assemblyName) then getLibraryPath assemblyName else getExecutablePath assemblyName)
        |> copyLibraryToPackagingDir
    
    let applicationName = 
        !! "*.sln"
            |> Seq.head
            |> filename
            |> fileNameWithoutExt 

    let frameworkVersion =
        getNonTestProjects
            |> Seq.map frameworkVersion
            |> Seq.map convertToNuGetFrameworkVersion
            |> Seq.max

    let packagesConfigByAllAndNoVersions =
        !! "**/packages.config"
            -- "**/*.Test*/**"
            |> Seq.toList
            |> Seq.collect getNugetDependencies
            |> Seq.distinctBy (fun dependency -> 
                let packageId, packageVersion, _ = dependency
                packageId.ToLower())
            |> Seq.groupBy (fun entry -> 
                let _, _, version = entry
                version)

    let nugetDependencies packageConfigs : (string * string) list =
        packageConfigs
        |> Seq.map (fun packageConfig -> 
            let packageId, packageVersion, _ = packageConfig
            packageId, packageVersion)
        |> Seq.toList

    let packagesConfigByVersions =
        packagesConfigByAllAndNoVersions
            |> Seq.where (fun entry -> fst entry <> "")

    let packagesConfigWithoutVersion =
        packagesConfigByAllAndNoVersions
            |> Seq.where (fun entry -> fst entry = "")
            |> Seq.collect (fun entry -> snd entry)
            |> nugetDependencies

    let dependencyByFramework (packageConfigByVersion:string * seq<string * string * string>) : NugetFrameworkDependencies =
        let version = fst packageConfigByVersion
        let packageConfigs = snd packageConfigByVersion
        {FrameworkVersion = version;
        Dependencies = nugetDependencies packageConfigs}

    NuGet (fun p ->
        {p with
            Authors = ["Kristian Hald"]
            Project = applicationName
            Description = "Builds .NET projects in all subfolders from provided input, downloads required NuGet packages and executes NUnit tests." // Must be defined by assembly info
            OutputPath = deployDir
            Summary = "" // Must be defined from the last official release
            WorkingDir = deployDir
            Version = version
            Publish = false
            Tags = "Build Tool NuGet NUnit" // Must be defined by assembly info
            Files = [("*.dll", Some ("lib/" + frameworkVersion), None)]
            Dependencies = packagesConfigWithoutVersion
            DependenciesByFramework = 
                packagesConfigByVersions
                |> Seq.map (fun packagesConfigByVersion -> dependencyByFramework packagesConfigByVersion)
                |> Seq.toList})
            "template.nuspec"

    !! (deployDir + "*.nupkg")
        |> Copy localFeedDir
)

Target "Default" (fun _ -> ()
)

// Dependencies
"Clean"
    ==> "BuildApp"
    ==> "BuildTest"
    ==> "Test"
    ==> "Default"
    ==> "LocalPackage"

// Start Build
RunTargetOrDefault "Default"