-module(test_basic_sup).
-behaviour(supervisor).

-export([init/1]).

%% A minimal one_for_one supervisor with a single transient child (a
%% test_counter_server started via gen_server). Used to exercise the
%% supervisor:terminate_child/2 and delete_child/2 bindings. `transient`
%% (rather than `temporary`) keeps the child spec around after
%% terminate_child/2, so delete_child/2 can then remove it.
init([]) ->
    ChildSpec = #{id => counter,
                  start => {gen_server, start_link, [test_counter_server, 0, []]},
                  restart => transient,
                  shutdown => 5000,
                  type => worker},
    {ok, {#{strategy => one_for_one}, [ChildSpec]}}.
