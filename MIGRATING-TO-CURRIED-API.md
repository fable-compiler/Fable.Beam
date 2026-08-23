# Migrating to the curried API

The current 5.0 release candidates make the public binding API consistently
curried. Functions take their configuration or input first and the value being
transformed last, which makes ordinary F# pipelines work naturally. The old
public `ImportAll` objects and their tupled calls are gone.

## Update calls

Replace module objects such as `maps` and tupled arguments with the module
function and curried arguments:

```fsharp
// Earlier release candidates
let map: BeamMap<string, int> = maps.new_ ()
let map = maps.put ("name", 42, map)
let name = maps.get ("name", map)

// Current curried API
module BeamMaps = Fable.Beam.Maps

let map: BeamMap<string, int> = BeamMaps.empty ()
let map = BeamMaps.put "name" 42 map
let name = BeamMaps.get "name" map
```

The collection is last, so common transformations can be piped:

```fsharp
let updated =
    BeamMaps.empty ()
    |> BeamMaps.put "name" (box "Ada")
    |> BeamMaps.put "active" (box true)
```

## Use semantic names

Where an OTP function used arity or an atom/boolean mode to select an operation,
the curried API gives each operation a descriptive name. Common String changes
are:

| Earlier release candidates | Current curried API |
| --- | --- |
| `str.slice (value, start, length)` | `BString.sliceLen value start length` |
| `str.trim (value, leading)` | `BString.trimStart value` |
| `str.trim (value, trailing)` | `BString.trimEnd value` |
| `str.equal (left, right, true)` | `BString.equalCaseInsensitive left right` |
| `str.pad (value, length, leading)` | `BString.padStart value length` |
| `str.pad (value, length, trailing)` | `BString.padEnd value length` |

```fsharp
module BString = Fable.Beam.String

let label = "  Ada  " |> BString.trimStart
let short = BString.sliceLen "Fable.Beam" 0 5
```

## Choose friendly or native values deliberately

The normal functions return familiar F# values. A `*Raw` function is available
where retaining Erlang's native list or chardata shape is useful to a BEAM
consumer. Use it only when that representation is required:

```fsharp
let text = BString.padEnd "Ada" 8
let chardata = BString.padEndRaw "Ada" 8
```

## Avoid FSharp.Core collisions

Keep the source-faithful module names, and alias them at the use site when they
would shadow FSharp.Core: `BeamMaps`, `BeamLists`, `BeamMath`, and `BString`.

## Pair JSX with Beam

`Fable.Beam.Jsx` has an explicit dependency on `Fable.Beam`. During the 5.0
release-candidate series, pair `Fable.Beam 5.0.0-rc.37` with
`Fable.Beam.Jsx 5.0.0-rc.9`. The package accepts `Fable.Beam >=
5.0.0-rc.37` and `< 6.0.0`, and NuGet restores a compatible core package.
