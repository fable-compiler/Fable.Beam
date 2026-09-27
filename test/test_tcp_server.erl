-module(test_tcp_server).

-export([start/0]).

start() ->
    {ok, ListenSocket} = gen_tcp:listen(0, [binary, {active, false}, {packet, line}, {reuseaddr, true}]),
    {ok, {_Address, Port}} = inet:sockname(ListenSocket),
    spawn(fun() -> serve(ListenSocket) end),
    Port.

serve(ListenSocket) ->
    {ok, Socket} = gen_tcp:accept(ListenSocket),
    ok = gen_tcp:close(ListenSocket),
    case gen_tcp:recv(Socket, 0, 2000) of
        {ok, Data} -> ok = gen_tcp:send(Socket, Data);
        {error, _Reason} -> ok
    end,
    gen_tcp:close(Socket).
