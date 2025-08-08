# Futures.Net

Primitives/utilities to write streameable async code.

### `Future<int, bool>`

```cs
new Future<int>()
    .Pipe(v => v.ToString())
    .Pipe(v => v == "1");
```

```cs
Future<int>
└─ Future<int, string>
   └─ Future<string, bool>
```