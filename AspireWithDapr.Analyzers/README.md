# AspireWithDapr.Analyzers

A .NET analyzer that validates classes implementing the Dapr Actor base class to ensure they follow proper serialization rules for both weakly-typed and strongly-typed Dapr Actor clients.

## Overview

This analyzer helps developers follow the serialization guidelines outlined in the [Dapr Actor Serialization documentation](https://docs.dapr.io/developing-applications/sdks/dotnet/dotnet-actors/dotnet-actors-serialization/) by providing compile-time validation and code fixes.

## Diagnostic Rules

### DAPR001 - Actor class should use DataContract attribute
**Severity:** Warning

Classes inheriting from `Actor` should be decorated with `[DataContract]` attribute for proper serialization with strongly-typed Dapr Actor clients.

**Bad:**
```csharp
public class WeatherActor : Actor
{
    public string Name { get; set; }
}
```

**Good:**
```csharp
[DataContract]
public class WeatherActor : Actor
{
    [DataMember]
    public string Name { get; set; }
}
```

### DAPR002 - Actor class properties should use DataMember attribute
**Severity:** Warning

Properties in Actor classes decorated with `[DataContract]` should use `[DataMember]` attribute for proper serialization.

**Bad:**
```csharp
[DataContract]
public class WeatherActor : Actor
{
    public string Name { get; set; } // Missing [DataMember]
}
```

**Good:**
```csharp
[DataContract]
public class WeatherActor : Actor
{
    [DataMember]
    public string Name { get; set; }
}
```

### DAPR003 - Actor class needs parameterless constructor or DataContract attribute
**Severity:** Error

Actor classes must have either a public parameterless constructor or be decorated with `[DataContract]` attribute for proper deserialization.

**Bad:**
```csharp
public class WeatherActor : Actor
{
    public WeatherActor(ActorHost host, string name) : base(host) 
    {
        Name = name;
    }
    
    public string Name { get; set; }
}
```

**Good (Option 1):**
```csharp
public class WeatherActor : Actor
{
    public WeatherActor() { } // Parameterless constructor
    
    public WeatherActor(ActorHost host, string name) : base(host) 
    {
        Name = name;
    }
    
    public string Name { get; set; }
}
```

**Good (Option 2):**
```csharp
[DataContract]
public class WeatherActor : Actor
{
    public WeatherActor(ActorHost host, string name) : base(host) 
    {
        Name = name;
    }
    
    [DataMember]
    public string Name { get; set; }
}
```

### DAPR004 - Actor interface should inherit from IActor
**Severity:** Error

Interfaces implemented by Actor classes should inherit from `IActor` interface.

**Bad:**
```csharp
public interface IWeatherActor
{
    Task<string> GetWeatherAsync();
}
```

**Good:**
```csharp
public interface IWeatherActor : IActor
{
    Task<string> GetWeatherAsync();
}
```

### DAPR005 - Enum members should use EnumMember attribute
**Severity:** Warning

Enum members used in Actor types should use `[EnumMember]` attribute for consistent serialization.

**Bad:**
```csharp
public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter
}
```

**Good:**
```csharp
public enum Season
{
    [EnumMember]
    Spring,
    [EnumMember]
    Summer,
    [EnumMember]
    Fall,
    [EnumMember]
    Winter
}
```

### DAPR006 - Consider using JsonPropertyName for property name consistency
**Severity:** Information

Properties in Actor classes used with weakly-typed clients should consider `[JsonPropertyName]` attribute for consistent property naming.

### DAPR007 - Complex types used in Actor methods need serialization attributes
**Severity:** Warning

Complex types used as parameters or return types in Actor methods should have proper serialization attributes.

## Installation

Install the analyzer via NuGet:

```xml
<PackageReference Include="AspireWithDapr.Analyzers" Version="1.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

## Configuration

You can configure the severity of each rule in your `.editorconfig` file:

```ini
[*.cs]
# Dapr Actor Analyzer Rules
dotnet_diagnostic.DAPR001.severity = warning
dotnet_diagnostic.DAPR002.severity = warning
dotnet_diagnostic.DAPR003.severity = error
dotnet_diagnostic.DAPR004.severity = error
dotnet_diagnostic.DAPR005.severity = warning
dotnet_diagnostic.DAPR006.severity = suggestion
dotnet_diagnostic.DAPR007.severity = warning
```

## Code Fixes

The analyzer provides automatic code fixes for:
- Adding `[DataContract]` attribute to Actor classes
- Adding `[DataMember]` attribute to properties
- Adding `[EnumMember]` attribute to enum members  
- Adding parameterless constructors to Actor classes

## Building

To build the analyzer:

```bash
dotnet build AspireWithDapr.Analyzers.csproj
```

To run tests:

```bash
dotnet test AspireWithDapr.Analyzers.Tests.csproj
```

## Contributing

When contributing to this analyzer, please ensure:
1. All new rules have corresponding tests
2. Code fixes are provided where applicable
3. Documentation is updated for new rules
4. Follow the existing code style and patterns

## License

This project is licensed under the MIT License.
