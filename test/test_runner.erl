-module(test_runner).
-export([main/1]).

%% Test runner for Fable.Beam tests.
%% Discovers every module exporting test_*/0 functions and runs them.
%%
%% Discovery is by exports, not by module name, on purpose: Fable >= 5.8 qualifies generated
%% Erlang module names with the OTP app name (test_maps.erl -> fable_beam_test_test_maps.erl),
%% which silently broke the old lists:prefix("test_", ...) match -- the suite then "passed"
%% while running zero tests. Hence also the empty-suite guard below.

main([Dir]) ->
    io:format("~n=== Fable.Beam Test Suite ===~n~n"),
    Beams = filelib:wildcard(filename:join(Dir, "*.beam")),
    Loaded = [begin
                  Mod = list_to_atom(filename:basename(F, ".beam")),
                  code:purge(Mod),
                  code:load_file(Mod),
                  Mod
              end || F <- Beams],
    Modules = [Mod || Mod <- Loaded, test_funs(Mod) =/= []],
    {TotalPass, TotalFail} = lists:foldl(
        fun(Mod, {AccPass, AccFail}) ->
            {P, F} = run_module(Mod),
            {AccPass + P, AccFail + F}
        end,
        {0, 0},
        lists:sort(Modules)
    ),
    Total = TotalPass + TotalFail,
    io:format("~n=== Results ===~n"),
    io:format("Total: ~p | Passed: ~p | Failed: ~p~n",
              [Total, TotalPass, TotalFail]),
    if
        Total =:= 0 ->
            io:format("~nNo tests found in ~s -- refusing to report success.~n", [Dir]),
            halt(1);
        TotalFail =:= 0 ->
            io:format("~nAll tests passed!~n"), ok;
        true ->
            io:format("~nSome tests FAILED!~n"), halt(1)
    end.

%% Exported arity-0 functions whose name starts with "test_".
test_funs(Mod) ->
    try
        [F || {F, 0} <- Mod:module_info(exports),
              F =/= module_info,
              lists:prefix("test_", atom_to_list(F))]
    catch
        _:_ -> []
    end.

run_module(Mod) ->
    io:format("--- ~s ---~n", [Mod]),
    lists:foldl(
        fun(Fun, {Pass, Fail}) ->
            case run_test(Mod, Fun) of
                pass -> {Pass + 1, Fail};
                fail -> {Pass, Fail + 1}
            end
        end,
        {0, 0},
        lists:sort(test_funs(Mod))
    ).

run_test(Mod, Fun) ->
    try
        Mod:Fun(),
        io:format("  \e[32m✓\e[0m ~s~n", [Fun]),
        pass
    catch
        %% Shape raised by fable_utils:assert_equal/assert_not_equal.
        error:#{message := Message, actual := Actual, expected := Expected} ->
            io:format("  \e[31m✗\e[0m ~s~n    ~ts~n    expected: ~p~n    actual:   ~p~n",
                      [Fun, Message, Expected, Actual]),
            fail;
        Class:Reason:Stack ->
            io:format("  \e[31m✗\e[0m ~s~n    ~p:~p~n    ~p~n", [Fun, Class, Reason, Stack]),
            fail
    end.
