# README

# Project: Integration Example

## Project Description

The goal is to prevent **duplicate records** during the integration of external item content into our system and to ensure consistency in **distributed system** scenarios.

The project addresses two scenarios:

1. **Single Server Scenario:**
   - Preventing the same item content from being recorded multiple times.
   - Allowing items with different content to be processed simultaneously.

2. **Distributed System Scenario:**
   - Preventing the same item content from being recorded multiple times when multiple servers are operating simultaneously.
   - Implementing a distributed locking mechanism using **Redis**.

## Project Structure

- **Integration.Common**
  - `Item` class: Defines the items.
  - `Result` class: Represents the outcome of the operations.

- **Integration.Backend**
  - `IItemOperationBackend` interface: Defines backend operations.
  - `ItemOperationBackend` class: Handles item saving and searching operations.

- **Integration.Service**
  - `ItemIntegrationService` class: The service layer where business logic resides. Implements the single server scenario.
  - `ItemIntegrationServiceDistributed` class: Implements the distributed system scenario.

- **Integration**
  - `ServiceCollectionExtensions` class: Extension method (DI) for registering services and dependencies.
  - `Program` class: The entry point of the application and where tests are conducted.

## Getting Started

### Requirements

- **.NET Core 6.0** or higher
- **Redis** server (for the distributed system scenario)
- **StackExchange.Redis** NuGet package (for the distributed system scenario)

### Cloning the Project

```bash
git clone https://github.com/Andronovo-bit/IntegrationScenarios.git
```

### Installing NuGet Packages

After opening the project, install the necessary NuGet packages:

```bash
dotnet restore
```

## Running the Project

1. Use the **single server** version of the **ItemIntegrationService** class (content-based locking mechanism).

2. Run the **Program.cs** file:

   ```bash
   dotnet run
   ```

3. In the console output, you will see that items with different content are recorded in parallel, and items with the same content are recorded only once.

**Sample Output:**

```
Everything recorded:
6:3
5:1
4:2
1:a
3:c
2:b
```

### ItemIntegrationService (Distributed System Scenario)

## Weaknesses and Points to Consider

- **Single Point of Failure:** If the Redis server crashes, the locking mechanism may fail.
- **Performance Impacts:** Distributed locking may affect performance due to network latency and additional processing load.
- **Complexity:** Managing locking and concurrency in distributed systems is more complex.
- **Scalability:** Redis server performance may degrade under high traffic.

You can access the detailed analysis [here](Integration/DistributedScenerioAnalyze.md).

## Dependency Injection and Service Lifetimes

- The project uses **Dependency Injection** to manage services and dependencies.
- Services are defined as **Singleton**, as they are designed to be thread-safe, and using the same instance throughout the application's lifetime optimizes resource usage.

**ServiceCollectionExtensions.cs:**

```csharp
public static IServiceCollection AddIntegrationServices(this IServiceCollection services)
{
    services.AddSingleton<IItemOperationBackend, ItemOperationBackend>();
    services.AddSingleton<ItemIntegrationService>();
    services.AddSingleton<ItemIntegrationDistributedService>();

    var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
    }
    return services;
}
```