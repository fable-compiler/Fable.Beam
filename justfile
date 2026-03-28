# Fable.Beam build commands
# Install just: https://github.com/casey/just

set dotenv-load

build_path := "build"
src_path := "src"
test_path := "test"

# Development mode: use local Fable repo instead of dotnet tool
# Usage: just dev=true test
dev := "false"
fable_repo := justfile_directory() / "../fable"
fable := if dev == "true" { "dotnet run --project " + fable_repo / "src/Fable.Cli" + " --" } else { "dotnet fable" }

# Default recipe - show available commands
default:
    @just --list

# Clean Fable build output
clean:
    rm -rf {{build_path}}
    rm -rf {{src_path}}/obj {{src_path}}/bin
    rm -rf {{test_path}}/obj {{test_path}}/bin
    rm -rf .fable

# Deep clean - removes everything including dotnet obj/bin directories
clean-all: clean
    rm -rf {{src_path}}/obj {{test_path}}/obj
    rm -rf {{src_path}}/bin {{test_path}}/bin

# Build F# source using dotnet
build:
    dotnet build {{src_path}}

# Transpile tests to Erlang and compile with rebar3
build-beam:
    dotnet build {{test_path}}
    {{fable}} {{test_path}} --lang Erlang --outDir {{build_path}}/tests
    cp {{test_path}}/test_runner.erl {{test_path}}/test_counter_server.erl {{build_path}}/tests/src/
    cp {{test_path}}/rebar.config {{build_path}}/tests/rebar.config
    cd {{build_path}}/tests && rebar3 compile

# Run BEAM tests (transpile F# to Erlang, compile, run on BEAM)
test: build-beam
    @echo ""
    cd {{build_path}}/tests && erl -noshell \
        -pa _build/default/lib/fable_beam_test/ebin \
        -pa _build/default/lib/fable_library_beam/ebin \
        -pa _build/default/lib/jsx/ebin \
        -eval 'test_runner:main(["_build/default/lib/fable_beam_test/ebin"])' \
        -s init stop

# Run only the dotnet build (verify F# compiles)
test-dotnet:
    dotnet build {{test_path}}
    dotnet run --project {{test_path}}

# Create NuGet packages with versions from changelogs
pack:
    #!/usr/bin/env bash
    set -euo pipefail
    get_version() { grep -m1 '^## ' "$1" | sed 's/^## \([^ ]*\).*/\1/'; }
    BEAM_VERSION=$(get_version src/otp/CHANGELOG.md)
    COWBOY_VERSION=$(get_version src/cowboy/CHANGELOG.md)
    JSX_VERSION=$(get_version src/jsx/CHANGELOG.md)
    dotnet pack src -c Release -o ./nupkgs -p:PackageVersion=$BEAM_VERSION -p:InformationalVersion=$BEAM_VERSION
    dotnet pack src/cowboy -c Release -o ./nupkgs -p:PackageVersion=$COWBOY_VERSION -p:InformationalVersion=$COWBOY_VERSION
    dotnet pack src/jsx -c Release -o ./nupkgs -p:PackageVersion=$JSX_VERSION -p:InformationalVersion=$JSX_VERSION

# Pack and push all packages to NuGet (used in CI)
release: pack
    dotnet nuget push './nupkgs/*.nupkg' -s https://api.nuget.org/v3/index.json -k $NUGET_KEY

# Format code with Fantomas
format:
    dotnet fantomas {{src_path}} -r
    dotnet fantomas {{test_path}} -r

# Check code formatting without making changes
format-check:
    dotnet fantomas {{src_path}} -r --check
    dotnet fantomas {{test_path}} -r --check

# Install .NET tools (Fable, Fantomas, etc.)
setup:
    dotnet tool restore

# Restore all dependencies
restore:
    dotnet paket install
    dotnet restore {{src_path}}
    dotnet restore {{test_path}}

# Watch for changes and rebuild (useful during development)
watch:
    {{fable}} watch {{src_path}} --lang Erlang --outDir {{build_path}}

# Run EasyBuild.ShipIt for release management
shipit *args:
    dotnet shipit --pre-release rc {{args}}
