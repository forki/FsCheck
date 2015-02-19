﻿namespace FsCheck

open Gen
open System
open System.Collections.Generic

///Extension methods to build generators - contains among other the Linq methods.
[<AbstractClass; Sealed; System.Runtime.CompilerServices.Extension>]
type GenExtensions = 

    ///Generates a value with maximum size n.
    //[category: Generating test values]
    [<System.Runtime.CompilerServices.Extension>]
    static member Eval(generator, size, random) =
        eval size random generator

    ///Generates n values of the given size.
    //[category: Generating test values]
    [<System.Runtime.CompilerServices.Extension>]
    static member Sample(generator, size, numberOfSamples) =
        sample size numberOfSamples generator

    ///Map the given function to the value in the generator, yielding a new generator of the result type.  
    [<System.Runtime.CompilerServices.Extension>]
    static member Select(g:Gen<_>, selector : Func<_,_>) = g.Map(fun a -> selector.Invoke(a))

    ///Generates a value that satisfies a predicate. This function keeps re-trying
    ///by increasing the size of the original generator ad infinitum.  Make sure there is a high chance that 
    ///the predicate is satisfied.
    [<System.Runtime.CompilerServices.Extension>]
    static member Where(g:Gen<_>, predicate : Func<_,_>) = suchThat (fun a -> predicate.Invoke(a)) g
    
    [<System.Runtime.CompilerServices.Extension>]
    static member SelectMany(source:Gen<_>, f:Func<_, Gen<_>>) = 
        gen { let! a = source
              return! f.Invoke(a) }
    
    [<System.Runtime.CompilerServices.Extension>]
    static member SelectMany(source:Gen<_>, f:Func<_, Gen<_>>, select:Func<_,_,_>) =
        gen { let! a = source
              let! b = f.Invoke(a)
              return select.Invoke(a,b) }

    ///Generates a list of given length, containing values generated by the given generator.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member ListOf (generator, nbOfElements) =
        listOfLength nbOfElements generator
        |> map (fun l -> new List<_>(l) :> IList<_>)

    /// Generates a list of random length. The maximum length depends on the
    /// size parameter.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member ListOf (generator) =
        listOf generator
        |> map (fun l -> new List<_>(l) :> IList<_>)

    /// Generates a non-empty list of random length. The maximum length 
    /// depends on the size parameter.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member NonEmptyListOf<'a> (generator) = 
        nonEmptyListOf generator 
        |> map (fun list -> new List<'a>(list) :> IList<_>)
    
    /// Generates an array of a specified length.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member ArrayOf (generator, length) =
        arrayOfLength length generator

    /// Generates an array using the specified generator. 
    /// The maximum length is size+1.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member ArrayOf (generator) =
        arrayOf generator

    /// Generates a 2D array of the given dimensions.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member Array2DOf (generator, rows, cols) =
        array2DOfDim (rows,cols) generator

    /// Generates a 2D array. The square root of the size is the maximum number of rows and columns.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member Array2DOf (generator) =
        array2DOf generator

    ///Apply the given Gen function to this generator, aka the applicative <*> operator.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member Apply (generator, f:Gen<Func<_,_>>) =
        apply (f |> map (fun f -> f.Invoke)) generator

    ///Override the current size of the test.
    //[category: Managing size]
    [<System.Runtime.CompilerServices.Extension>]
    static member Resize (generator, newSize) =
        resize newSize generator

    /// Construct an Arbitrary instance from a generator.
    /// Shrink is not supported for this type.
    [<System.Runtime.CompilerServices.Extension>]
    static member ToArbitrary generator =
        Arb.fromGen generator

    /// Construct an Arbitrary instance from a generator and a shrinker.
    [<System.Runtime.CompilerServices.Extension>]
    static member ToArbitrary (generator,shrinker) =
        Arb.fromGenShrink (generator,shrinker)

    ///Build a generator that generates a 2-tuple of the values generated by the given generator.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member Two (generator) =
        two generator

    ///Build a generator that generates a 3-tuple of the values generated by the given generator.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member Three (generator) =
        three generator

    ///Build a generator that generates a 4-tuple of the values generated by the given generator.
    //[category: Creating generators from generators]
    [<System.Runtime.CompilerServices.Extension>]
    static member Four (generator) =
        four generator

