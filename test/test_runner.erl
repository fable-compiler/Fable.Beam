-module(test_runner).
-export([main/1]).

%% Test runner for Fable.Beam tests.
%% Discovers modules starting with "test_" and runs all test_*/0 functions.

main([Dir]) ->
    io:format("~n=== Fable.Beam Test Suite ===~n~n"),
    Beams = filelib:wildcard(filename:join(Dir, "*.beam")),
    Modules = [list_to_atom(filename:basename(F, ".beam")) || F <- Beams,
               lists:prefix("test_", filename:basename(F, ".beam"))],
    {TotalPass, TotalFail} = lists:foldl(
        fun(Mod, {AccPass, AccFail}) ->
            code:purge(Mod),
            code:load_file(Mod),
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
    case TotalFail of
        0 -> io:format("~nAll tests passed!~n"), ok;
        _ -> io:format("~nSome tests FAILED!~n"), halt(1)
    end.

run_module(Mod) ->
    io:format("--- ~s ---~n", [Mod]),
    Exports = Mod:module_info(exports),
    TestFuns = [F || {F, 0} <- Exports,
                F =/= module_info,
                lists:prefix("test_", atom_to_list(F))],
    lists:foldl(
        fun(Fun, {Pass, Fail}) ->
            case run_test(Mod, Fun) of
                pass -> {Pass + 1, Fail};
                fail -> {Pass, Fail + 1}
            end
        end,
        {0, 0},
        lists:sort(TestFuns)
    ).

run_test(Mod, Fun) ->
    try
        Mod:Fun(),
        io:format("  \e[32m✓\e[0m ~s~n", [Fun]),
        pass
    catch
        error:{assertEqual, Props} ->
            Expected = proplists:get_value(expected, Props),
            Actual = proplists:get_value(value, Props),
            io:format("  \e[31m✗\e[0m ~s~n    expected: ~p~n    actual:   ~p~n", [Fun, Expected, Actual]),
            fail;
        Class:Reason:Stack ->
            io:format("  \e[31m✗\e[0m ~s~n    ~p:~p~n    ~p~n", [Fun, Class, Reason, Stack]),
            fail
    end.
