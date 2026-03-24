module Fable.Beam.Tests.Logger

open Fable.Beam.Testing

#if FABLE_COMPILER
open Fable.Beam.Logger
#endif

[<Fact>]
let ``test logger.info works`` () =
#if FABLE_COMPILER
    logger.info "test info message"
#else
    ()
#endif

[<Fact>]
let ``test logger.warning works`` () =
#if FABLE_COMPILER
    logger.warning "test warning message"
#else
    ()
#endif

[<Fact>]
let ``test logger.debug works`` () =
#if FABLE_COMPILER
    logger.debug "test debug message"
#else
    ()
#endif

[<Fact>]
let ``test logger.info with format args`` () =
#if FABLE_COMPILER
    // The 2-arg overload accepts both metadata maps and format args lists
    logger.info ("test ~p message", box [ box 42 ])
#else
    ()
#endif
