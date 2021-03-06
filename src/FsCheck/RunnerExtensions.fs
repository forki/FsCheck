﻿namespace FsCheck

open System
open System.Runtime.CompilerServices

///Configure the test run.
type Configuration() =
    let mutable maxTest = Config.Quick.MaxTest
    let mutable maxFail = Config.Quick.MaxFail
    let mutable name = Config.Quick.Name
    let mutable every = Config.Quick.Every
    let mutable everyShrink = Config.Quick.EveryShrink
    let mutable startSize = Config.Quick.StartSize
    let mutable endSize = Config.Quick.EndSize
    let mutable runner = Config.Quick.Runner
    let mutable replay = Config.Quick.Replay

    ///The maximum number of tests that are run.
    member __.MaxNbOfTest with get() = maxTest and set(v) = maxTest <- v

    ///The maximum number of tests where values are rejected
    member __.MaxNbOfFailedTests with get() = maxFail and set(v) = maxFail <- v

    ///Name of the test.
    member __.Name with get() = name and set(v) = name <- v

    ///What to print when new arguments args are generated in test n
    member __.Every with get() = new Func<int,obj array,string>(fun i arr -> every i (Array.toList arr)) 
                    and set(v:Func<int,obj array,string>) = every <- fun i os -> v.Invoke(i,List.toArray os)

    ///What to print every time a counter-example is succesfully shrunk
    member __.EveryShrink with get() = new Func<obj array,string>(Array.toList >> everyShrink)
                          and set(v:Func<obj array,string>) = everyShrink <- fun os -> v.Invoke(List.toArray os)

    ///The size to use for the first test.
    member __.StartSize with get() = startSize and set(v) = startSize <- v

    ///The size to use for the last test, when all the tests are passing. The size increases linearly between Start- and EndSize.
    member __.EndSize with get() = endSize and set(v) = endSize <- v

    ///A custom test runner, e.g. to integrate with a test framework like xUnit or NUnit. 
    member __.Runner with get() = runner and set(v) = runner <- v

    //TODO: figure out how to deal with null values
    //member __.Replay with get() = (match replay with None -> null | Some (Random.StdGen s) -> s) and set(v) = replay <- Some (Random.StdGen v)
    member internal __.ToConfig() =
        { MaxTest = maxTest
          MaxFail = maxFail 
          Name = name
          Every = every
          EveryShrink = everyShrink
          StartSize = startSize
          EndSize = endSize
          Runner = runner
          Replay = replay
          Arbitrary = []
        }

[<AbstractClass;Sealed;Extension>]
type SpecificationExtensions private() =
    /// Check one property with the quick configuration
    [<Extension>]
    static member QuickCheck(spec:Specification) = Check.Quick(spec.Build())

    /// Check one property with the quick configuration, and throw an exception if it fails or is exhausted.
    [<Extension>]
    static member QuickCheckThrowOnFailure(spec:Specification) = Check.QuickThrowOnFailure(spec.Build())

    /// Check one property with the verbose configuration.
    [<Extension>]
    static member VerboseCheck(spec:Specification) = Check.Verbose(spec.Build())

    /// Check one property with the verbose configuration, and throw an exception if it fails or is exhausted.
    [<Extension>]
    static member VerboseCheckThrowOnFailure(spec:Specification) = Check.VerboseThrowOnFailure(spec.Build())

    /// Check one property with the quick configuration, and using the given name.
    [<Extension>]
    static member QuickCheck(spec:Specification, name:string) = Check.Quick(name,spec.Build())

    /// Check one property with the quick configuration, and throw an exception if it fails or is exhausted.
    [<Extension>]
    static member QuickCheckThrowOnFailure(spec:Specification, name:string) = Check.QuickThrowOnFailure(name,spec.Build())

    ///Check one property with the verbose configuration, and using the given name.
    [<Extension>]
    static member VerboseCheck(spec:Specification, name:string) = Check.Verbose(name,spec.Build())

    /// Check one property with the verbose configuration, and throw an exception if it fails or is exhausted.
    [<Extension>]
    static member VerboseCheckThrowOnFailure(spec:Specification, name:string) = Check.VerboseThrowOnFailure(name,spec.Build())

    ///Check the given property using the given config.
    [<Extension>]
    static member Check(spec:Specification, configuration:Configuration) = Check.One(configuration.ToConfig(),spec.Build())
