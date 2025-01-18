# Wrapture

**Wrapture** is a lightweight .NET library designed to bring functional programming abstractions to your applications. By using core types like `Result`, `Maybe`, `Either`, `Specification`, and utilities for pagination, Wrapture helps you write clean, declarative, and composable code.

---

## Features

- **Result**: Encapsulate success or failure states with an expressive API.
- **Maybe**: Represent optional values without relying on `null`.
- **Either**: Handle computations with two possible outcomes, such as success or error.
- **Specification**: Encapsulate business rules and make them reusable and composable.
- **Pagination**: Simplify handling paginated data with `PagedResult`.

---

## Installation

You can install Wrapture via NuGet:

```bash
Install-Package Wrapture
```

Or using the .NET CLI:

```bash
dotnet add package Wrapture
```

---

## Getting Started

### Example: Using `Result`

```csharp
var result = ProcessData(input)
    .Map(Transform)
    .Bind(Validate);

result.Match(
    success => Console.WriteLine($"Success: {success}"),
    error => Console.WriteLine($"Error: {error}")
);
```

### Example: Using `Maybe`

```csharp
var maybeValue = FetchOptionalValue();

maybeValue.Match(
    some => Console.WriteLine($"Value: {some}"),
    none => Console.WriteLine("No value present")
);
```

### Example: Using `Either`

```csharp
var either = ParseNumber("123");

either.Match(
    left => Console.WriteLine($"Error: {left}"),
    right => Console.WriteLine($"Parsed value: {right}")
);
```

---

## Contributing

Contributions are welcome! Feel free to submit issues or pull requests on [GitHub](https://github.com/NyronW/Wrapture).

---

## License

This project is licensed under the [MIT License](LICENSE).
