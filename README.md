# MutableOptions

Extends [.NET Options pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options) to support writing back to [IConfiguration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration) while mutating strongly-typed options.

### Usage
#### Register in [dependency injection service container](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
`ConfigureMutable` method is provided, that basically can be used instead of [`Configure`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.optionsconfigurationservicecollectionextensions) for options that needs to support mutations:
```csharp
services.ConfigureMutable<SimpleOptions>(configurationRoot.GetSection("SimpleOptions"));
```
#### Resolve `IOptionsMutator<SimpleOptions>` to modify the configuration:
```csharp
IOptionsMutator<SimpleOptions> optionsMutator
optionsMutator.Mutate(options => options with { Bar = 42 });
```
For convenience in situations when same code have to read and write options there is `IMutableOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>, IOptionsMutator<TOptions>` interface.
Both `IOptionsMutator<TOptions>` and `IMutableOptionsMonitor<TOptions>` are registered with singleton lifetime.

### Installation
Install [NuGet package](https://www.nuget.org/packages/MutableOptions/) from [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console):
```
PM> Install-Package MutableOptions
```

### Features
- Compatible with all standartd Options patter interfaces, e.g. `IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`, IConfigureOptions<T> etc.
- Named options are supported. Specify `name` parameter to `IOptionsMutator.Mutate` method to change value of named options.
- Supports [`BinderOptions`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.binderoptions?view=dotnet-plat-ext-6.0) where it could be specified whether it can detect changed in non-public properties and white them to `IConfiguration`.
- Can write values of most primitive types properties in flat options types.
***
**NOTE:** Complex hierarchical options types, that contain nested classes or collections, is not supported!
***


### Design considerations

[IOptionsMutator](https://github.com/dombrovsky/MutableOptions/blob/main/MutableOptions/IOptionsMutator.cs) is a core interface that allows changing configuration by mutating strongly typed options:
```csharp
public interface IOptionsMutator<TOptions> where TOptions : class
{
    bool Mutate(string? name, Func<TOptions, TOptions> mutator);
}
```
, where `mutator` func should return new `TOptions` instance based on `TOptions` instance provided, changing the values of some or all properties.

It is recommended to define options types as immutable types, so it would not be possible to change state of `TOptions` instance directly.
The easiest way to achieve that is to use [records](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record) with init-only properties, e.g.:
```csharp
public record SimpleOptions
{
    public string Foo { get; init; } = string.Empty;
    
    public int Bar { get; init; }
}
```
Record types also implement `IEquatable<T>`, which allows to configure mutable options without specifying custom `IEqualityComparer<T>`.
Another usefult capability of record types is that they have support for `with` expressions to enable non-destructive mutation of records, which can be used to simplify `mutator` function implementtion.
```csharp
optionsMutator.Mutate(options => options with { Bar = 42 });
```

