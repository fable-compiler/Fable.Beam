-module(test_counter_server).
-behaviour(gen_server).

-export([init/1, handle_call/3, handle_cast/2]).

init(InitialCount) ->
    {ok, InitialCount}.

handle_call(get, _From, State) ->
    {reply, State, State};
handle_call(increment, _From, State) ->
    {reply, State + 1, State + 1};
handle_call({add, N}, _From, State) ->
    {reply, State + N, State + N};
handle_call(_Request, _From, State) ->
    {reply, {error, unknown_request}, State}.

handle_cast({set, N}, _State) ->
    {noreply, N};
handle_cast(_Request, State) ->
    {noreply, State}.
