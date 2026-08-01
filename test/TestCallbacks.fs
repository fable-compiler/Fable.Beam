/// Regression tests for how F# function values cross into Erlang.
///
/// These pin the codegen contract that BINDINGS-GUIDE.md ("Callbacks: plain F#
/// function types work at any arity") relies on: an F# function value compiles to an
/// Erlang fun of its *remaining* arity, so a curried `'T -> 'Acc -> 'Acc` reaches
/// `lists:foldl/3` as the 2-arity fun it applies as `F(Elem, Acc)` — no `System.Func`
/// wrapper needed. If a future Fable release stops eta-expanding, these fail with
/// `badarity` and the guide's recommendation needs revisiting.
module Fable.Beam.Tests.Callbacks

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.BeamInterop
open Fable.Beam

/// Curried 2-argument callback, handed straight to lists:foldl/3.
[<Emit("lists:foldl($0, $1, $2)")>]
let private foldlCurried (f: 'T -> 'Acc -> 'Acc) (acc: 'Acc) (l: Lists.BeamList<'T>) : 'Acc = nativeOnly

/// Same binding declared with System.Func — must behave identically.
[<Emit("lists:foldl($0, $1, $2)")>]
let private foldlFunc (f: System.Func<'T, 'Acc, 'Acc>) (acc: 'Acc) (l: Lists.BeamList<'T>) : 'Acc = nativeOnly

/// The callback's arity is hidden behind `obj` at the binding.
[<Emit("lists:foldl($0, $1, $2)")>]
let private foldlObj (f: obj) (acc: 'Acc) (l: Lists.BeamList<'T>) : 'Acc = nativeOnly

// fsharplint:disable MemberNames
/// A tupled [<ImportAll>] interface member with curried callbacks — the shape
/// Lists/Maps/Queue use.
[<Erase>]
type private ICallbackProbe =
    abstract foldl: f: ('T -> 'Acc -> 'Acc) * acc: 'Acc * list: Lists.BeamList<'T> -> 'Acc
    abstract filter: pred: ('T -> bool) * list: Lists.BeamList<'T> -> Lists.BeamList<'T>

[<ImportAll("lists")>]
let private probeLists: ICallbackProbe = nativeOnly

let private addFn (x: int) (acc: int) : int = x + acc

let private makeAdder () : int -> int -> int = fun x acc -> x + acc

let private add3 (a: int) (b: int) (c: int) : int = a + b + c

let private nums () : Lists.BeamList<int> = emitErlExpr () "[1, 2, 3]"
#endif

[<Fact>]
let ``test curried lambda literal reaches foldl as a 2-arity fun`` () =
#if FABLE_COMPILER
    foldlCurried (fun x acc -> x + acc) 0 (nums ()) |> equal 6
#else
    ()
#endif

[<Fact>]
let ``test System.Func callback behaves identically to the curried form`` () =
#if FABLE_COMPILER
    foldlFunc (System.Func<_, _, _>(fun x acc -> x + acc)) 0 (nums ()) |> equal 6
#else
    ()
#endif

[<Fact>]
let ``test named curried function reaches foldl as a 2-arity fun`` () =
#if FABLE_COMPILER
    foldlCurried addFn 0 (nums ()) |> equal 6
#else
    ()
#endif

[<Fact>]
let ``test curried function returned from a function keeps its arity`` () =
#if FABLE_COMPILER
    // Arity is not syntactically visible at the call site.
    foldlCurried (makeAdder ()) 0 (nums ()) |> equal 6
#else
    ()
#endif

[<Fact>]
let ``test partially applied function passes its remaining arity`` () =
#if FABLE_COMPILER
    // add3 10 has two arguments left, so it must arrive as a 2-arity fun.
    foldlCurried (add3 10) 0 (nums ()) |> equal 36
#else
    ()
#endif

[<Fact>]
let ``test callback boxed through an obj-typed parameter keeps its arity`` () =
#if FABLE_COMPILER
    foldlObj (box (fun x acc -> x + acc)) 0 (nums ()) |> equal 6
#else
    ()
#endif

[<Fact>]
let ``test ImportAll interface member takes a curried 2-arg callback`` () =
#if FABLE_COMPILER
    probeLists.foldl ((fun x acc -> x + acc), 0, nums ()) |> equal 6
#else
    ()
#endif

[<Fact>]
let ``test ImportAll interface member takes a curried 1-arg callback`` () =
#if FABLE_COMPILER
    let kept = probeLists.filter ((fun x -> x > 1), nums ())
    let n: int = emitErlExpr kept "erlang:length($0)"
    n |> equal 2
#else
    ()
#endif
