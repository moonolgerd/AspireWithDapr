# AspireWithDapr - AI Coding Agent Instructions

## Architecture Overview

This is a .NET Aspire application demonstrating Dapr Virtual Actors and Pub/Sub patterns with horizontal scaling. The system processes weather forecasts through a distributed architecture with three main services.

## Core Components

### 1. AppHost (`AspireWithDapr.AppHost/Program.cs`)
- **Purpose**: Orchestrates the entire distributed application using .NET Aspire
- **Key Pattern**: Uses `builder.AddDaprStateStore()` and `builder.AddDaprPubSub()` with local YAML component files
- **Service Dependencies**: Uses `.WaitFor(cache)` to ensure Redis is ready before starting dependent services

### 2. API Service (`AspireWithDapr.ApiService/`)
- **Purpose**: Weather Actor host with GraphQL endpoint and Dapr pub/sub subscriber
- **Key Files**:
  - `WeatherActor.cs`: Stateful Dapr actor that manages city-specific weather data using `StateManager`
  - `Query.cs`: GraphQL query resolver using actor proxy pattern
  - `Subscription.cs`: Real-time GraphQL subscriptions via HotChocolate + Redis
- **Actor Pattern**: Uses `ActorId(city)` for city-based partitioning
- **Scaling**: Configured with `.WithReplicas(3)` in AppHost

### 3. Web Frontend (`AspireWithDapr.Web/`)
- **Purpose**: Blazor Server UI consuming the API service
- **Integration**: Uses `WeatherApiClient` to communicate with API service via service discovery
- **Caching**: Redis output caching with `builder.AddRedisOutputCache("cache")`

### 4. Publisher (`AspireWithDapr.Publisher/`)
- **Purpose**: Background service generating weather data via Dapr pub/sub
- **Pattern**: `PublisherHostedService` publishes to `SharedConstants.TopicName` using `DaprClient.PublishEventAsync()`

## Critical Developer Workflows

### Running the Application
```bash
aspire run
```
- Access Aspire Dashboard at https://localhost:17174
- Dapr Dashboard available at http://localhost:9999

### Dapr Component Configuration
- **State Store**: `statestore.yaml` - Redis-backed actor state with password authentication
- **Pub/Sub**: `pubsub.yaml` - Redis streams for weather forecast distribution

### Service Communication Patterns
- **Actor-to-Actor**: Via `IActorProxyFactory.CreateActorProxy<IWeatherActor>()`
- **Pub/Sub**: Publisher → Topic → API Service subscriber endpoint `/weatherforecast` with `[FromBody]` and `.WithTopic()`
- **GraphQL Subscriptions**: Real-time updates using `ITopicEventSender.SendAsync()` in actors

## Project-Specific Conventions

### Shared Constants (`AspireWithDapr.Shared/`)
- Use `SharedConstants.PubsubName` and `SharedConstants.TopicName` for pub/sub communication
- City names from `SharedCollections.Cities` ensure consistent data across services
- `WeatherForecast` record is the primary data contract

### Service Integration
- All services use `builder.AddServiceDefaults()` for OpenTelemetry, health checks, and service discovery
- Redis client registration: `builder.AddRedisClient("Redis")` with connection name matching AppHost definition
- Dapr integration: `builder.Services.AddDaprClient()` for pub/sub, `AddActors()` for actor hosting

### GraphQL + Dapr Integration
- **Query Pattern**: Direct actor proxy calls in resolvers (`Query.GetWeatherForecastsAsync`)
- **Subscription Pattern**: Uses HotChocolate's `[Subscribe(With = nameof(SubscribeAsync))]` attribute
- **Real-time Updates**: Actors publish to GraphQL subscriptions via `ITopicEventSender`

## Development Notes

### Actor State Management
- State persisted per city using `StateManager.SetStateAsync(Id.GetId(), forecasts)`
- Actor ID equals city name for natural partitioning
- Use `StateManager.TryGetStateAsync<T>()` for safe state retrieval

### Error Handling
- Actors should handle duplicate forecast prevention: `if (list.Contains(forecast)) return;`

### Testing Scaled Scenarios
- API service runs with 3 replicas by default
- Monitor actor distribution across instances via logs and Dapr dashboard

When modifying this codebase, maintain the actor-per-city pattern, respect the pub/sub flow (Publisher → API Service → GraphQL subscriptions), and ensure Redis authentication consistency across Aspire parameters and Dapr component configurations.
