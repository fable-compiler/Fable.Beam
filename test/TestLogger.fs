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
