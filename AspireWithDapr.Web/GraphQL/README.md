# GraphQL Integration Summary

## Overview
The Web project now uses **StrawberryShake v15.1.11** - a fully-fledged GraphQL client with complete code generation. This provides compile-time type safety, IntelliSense support, and integrates seamlessly with the HotChocolate GraphQL server running in the ApiService.

## Components Added

### 1. StrawberryShake Generated Client (`AspireWithDapr.Web.Generated` namespace)
The code generator creates:
- **Type-safe query operations**: `IGetWeatherForecastsQuery` with `ExecuteAsync` and `Watch` methods
- **Subscription operations**: `IOnWeatherForecastSubscription` for real-time weather updates via WebSockets
- **Mutation operations**: `IAddWeatherForecastMutation` for adding weather forecasts
- **Result types**: Strongly-typed `IGetWeatherForecastsResult`, `IGetWeatherForecasts_WeatherForecasts` interfaces
- **Client interface**: `IWeatherApiClient` aggregating all operations

### 2. Code Generation Configuration

**`.graphqlrc.json`**:
```json
{
  "schema": "schema.graphql",
  "documents": "GraphQL/**/*.graphql",
  "extensions": {
    "strawberryShake": {
      "name": "WeatherApiClient",
      "namespace": "AspireWithDapr.Web.Generated",
      "url": "http://api/graphql",
      "dependencyInjection": true,
      "entityRecords": true,
      "strictSchemaValidation": false
    }
  }
}
```

**`GraphQL/WeatherOperations.graphql`**:
```graphql
query GetWeatherForecasts($city: String!) {
  weatherForecasts(city: $city) {
    city date temperatureC temperatureF summary
  }
}

subscription OnWeatherForecast($city: String!) {
  onWeatherForecast(city: $city) {
    city date temperatureC temperatureF summary
  }
}

mutation AddWeatherForecast($forecast: WeatherForecastInput!) {
  addWeatherForecast(forecast: $forecast) {
    city date temperatureC temperatureF summary
  }
}
```

### 3. Updated Program.cs
```csharp
builder.Services
    .AddWeatherApiClient()
    .ConfigureHttpClient((sp, client) => {
        client.BaseAddress = new Uri("http://apiservice/graphql");
    })
    .ConfigureWebSocketClient((sp, client) => {
        client.Uri = new Uri("ws://apiservice/graphql");
    });
```

### 4. Updated Weather.razor
- Injects `IWeatherApiClient` instead of manual GraphQL client
- Uses strongly-typed `result.Data.WeatherForecasts`
- Type-safe access to `Date`, `TemperatureC`, `TemperatureF`, `Summary` properties
- Proper error handling with `result.Errors`

## Code Generation Workflow

### Generate Client Code
```bash
dotnet graphql generate
```

### Local Tool Installation
The project uses a local dotnet tool manifest (`.config/dotnet-tools.json`):
```bash
dotne
