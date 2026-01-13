# How to use Bryntum Calendar with .NET and SQLite

[Bryntum Calendar](https://bryntum.com/products/calendar/) is a performant, highly customizable JavaScript UI component 
with multiple views. It integrates with the major JavaScript web frameworks. This tutorial demonstrates how to use 
Bryntum Calendar with a [.NET Framework](https://dotnet.microsoft.com/en-us/) backend and SQLite.

You'll learn to do the following:

- Set up a .NET Web API that uses a local SQLite database and [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/).
- Configure Entity Framework Core models to define the database table structure.
- Run a seed command to populate the database with example JSON data.
- Create API endpoints to load data and sync data changes to the database.
- Set up a ReactBryntum Calendar frontend using TypeScript and Vite.
- Configure the Bryntum Calendar to load data from the database and synchronize changes to the database
  using the created API endpoints.

Here's what we'll build:

![Bryntum Calendar](images/bryntum-calendar-complete.png)

## Prerequisites

To follow along, you need the [.NET SDK](https://dotnet.microsoft.com/en-us/download) (version 10.0 or later) and 
[Node.js](https://nodejs.org/en/download) installed on your system.

## Getting started

We'll use a starter project for the .NET backend and the Vite TypeScript frontend.

### Backend starter

Clone the [.NET starter GitHub repository](https://github.com/ritza-co/bryntum-calendar-dotnet-starter). The code for 
the completed tutorial is in the 
[`completed-app`](https://github.com/ritza-co/bryntum-calendar-dotnet-starter/tree/completed-app) branch 
of the repository.

The .NET app has the following directory structure:

- `Program.cs` sets up and starts an ASP.NET Core Web API with a single "Hello World" endpoint.
- `dotnet-sqlite-calendar.csproj` is the project file that defines dependencies and project settings.
- `appsettings.json` is the application's configuration file.
- `example-data` contains the example events and resources for a Bryntum Calendar, stored as  JSON data. We'll use this 
  data to populate a local SQLite database.

Follow the instructions in the `README.md` file to install the dependencies.

### Frontend starter

Clone the [Bryntum Calendar starter GitHub repository](https://github.com/ritza-co/bryntum-calendar-vanilla-typescript-starter). 
The code for the completed tutorial is in the [`completed-app`](https://github.com/ritza-co/bryntum-calendar-vanilla-typescript-starter/tree/completed-app) 
branch of the repository.

Follow the instructions in the `README.md` file to install the dependencies and run the app.

## Configure the .NET SQLite database connection

In the backend starter, update the `appsettings.json` file to add a connection string below the `"AllowedHosts"` key, 
for a local SQLite database:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=calendar.sqlite3"
}
```

This string names the database file `calendar.sqlite3` and stores it in the project directory.

## Create the data models

We'll define database models for the events and resources example data using [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/). 
In Bryntum Calendar, data stores are kept and linked 
together in the [project](https://bryntum.com/products/calendar/docs/guide/Calendar/data/displayingdata#the-calendar-project). 
Bryntum Calendar uses the following data stores:  

- [ResourceStore](https://bryntum.com/products/calendar/docs/api/Scheduler/data/ResourceStore)
- [EventStore](https://bryntum.com/products/calendar/docs/api/Scheduler/data/EventStore)
- [AssignmentStore](https://bryntum.com/products/calendar/docs/api/Scheduler/data/AssignmentStore)
- [TimeRangeStore](https://bryntum.com/products/calendar/docs/api/Scheduler/data/TimeRangeStore)
- [ResourceTimeRangeStore](https://bryntum.com/products/calendar/docs/api/Scheduler/data/ResourceTimeRangeStore)

This basic tutorial covers making models for the EventStore and the ResourceStore.

### Create the Event model

Create a folder called `Models` in the project directory. Create a file called `Event.cs` in this folder and add the 
following lines of code to it:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CalendarApi.Models
{
    [Table("events")]
    public class Event
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("$PhantomId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
        public string? PhantomId { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [Column("startDate")]
        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [Column("endDate")]
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [Column("allDay")]
        [JsonPropertyName("allDay")]
        public bool? AllDay { get; set; } = false;

        [Column("resourceId")]
        [JsonPropertyName("resourceId")]
        public string? ResourceId { get; set; }

        [Column("eventColor")]
        [JsonPropertyName("eventColor")]
        public string? EventColor { get; set; }

        [Column("readOnly")]
        [JsonPropertyName("readOnly")]
        public bool? ReadOnly { get; set; } = false;

        [Column("timeZone")]
        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }

        [Column("draggable")]
        [JsonPropertyName("draggable")]
        public bool? Draggable { get; set; } = true;

        [Column("resizable")]
        [JsonPropertyName("resizable")]
        public string? Resizable { get; set; } = "true";

        [Column("duration")]
        [JsonPropertyName("duration")]
        public double? Duration { get; set; }

        [Column("durationUnit")]
        [JsonPropertyName("durationUnit")]
        public string? DurationUnit { get; set; } = "day";

        [Column("exceptionDates")]
        [JsonPropertyName("exceptionDates")]
        [JsonConverter(typeof(JsonStringToArrayConverter))]
        public string? ExceptionDates { get; set; }

        [Column("recurrenceRule")]
        [JsonPropertyName("recurrenceRule")]
        public string? RecurrenceRule { get; set; }

        [Column("cls")]
        [JsonPropertyName("cls")]
        public string? Cls { get; set; }

        [Column("eventStyle")]
        [JsonPropertyName("eventStyle")]
        public string? EventStyle { get; set; }

        [Column("iconCls")]
        [JsonPropertyName("iconCls")]
        public string? IconCls { get; set; }

        [Column("style")]
        [JsonPropertyName("style")]
        public string? Style { get; set; }
    }
}
```

We define the `Event` model class that represents the `"events"` table in the database. The table name is set using the 
`[Table("events")]` attribute.

The model properties define the columns for the database table. We use [data annotations](https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties) 
to set the column names, data types, and constraints. The `[JsonPropertyName]` attributes ensure the properties are 
serialized to the correct JSON property names expected by the Bryntum Calendar.

The `ExceptionDates` property uses a custom JSON converter, which we'll create later, to convert the string stored in 
the database to a JSON array for the Bryntum Calendar.

### Create the Resource model

Create a file called `Resource.cs` in the `Models` directory and add the following lines of code to it:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CalendarApi.Models
{
    [Table("resources")]
    public class Resource
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("$PhantomId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
        public string? PhantomId { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [Column("eventColor")]
        [JsonPropertyName("eventColor")]
        public string? EventColor { get; set; }

        [Column("readOnly")]
        [JsonPropertyName("readOnly")]
        public bool? ReadOnly { get; set; } = false;
    }
}
```

We define the `Resource` model class that represents the `"resources"` table in the database. 

### Create the JSON string-to-array converter

The Bryntum Calendar sends and expects `exceptionDates` as a JSON array, but we store it as a string in SQLite.

Create a file called `JsonStringToArrayConverter.cs` in the `Models` directory to handle this conversion. 
Add the following code to it:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalendarApi.Models
{
    /// <summary>
    /// Converts a JSON string stored in the database to an array when serializing for API responses.
    /// E.g., stored as "[]" or "[\"2025-01-01\"]" -> serialized as [] or ["2025-01-01"]
    /// </summary>
    public class JsonStringToArrayConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // When reading from JSON (e.g., from request), we might get an array or string
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Read the array and convert to JSON string for storage
                using var doc = JsonDocument.ParseValue(ref reader);
                return doc.RootElement.GetRawText();
            }
            
            // If it's already a string, return as-is
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            // Parse the JSON string and write it as raw JSON (array)
            try
            {
                using var doc = JsonDocument.Parse(value);
                doc.RootElement.WriteTo(writer);
            }
            catch
            {
                // If parsing fails, write as null
                writer.WriteNullValue();
            }
        }
    }
}
```

This custom JSON converter converts a JSON string stored in the database to an array when serializing for API responses.

### Create the sync request and response models

The Bryntum Calendar has a [Crud Manager](https://bryntum.com/products/calendar/docs/guide/Calendar/data/crud_manager) 
that simplifies loading data from and syncing data changes to the .NET backend. This Crud Manager uses 
a specific [sync request structure](https://bryntum.com/products/calendar/docs/guide/Scheduler/data/crud_manager_in_depth#sync-request-structure) 
for data synchronization.

Create a file called `SyncModels.cs` in the `Models` directory and add the following code to it:

```csharp
using System.Text.Json.Serialization;

namespace CalendarApi.Models
{
    // Request DTOs
    public class SyncRequest
    {
        [JsonPropertyName("requestId")]
        public long? RequestId { get; set; }

        [JsonPropertyName("events")]
        public StoreChanges<Event>? Events { get; set; }

        [JsonPropertyName("resources")]
        public StoreChanges<Resource>? Resources { get; set; }
    }

    public class StoreChanges<T>
    {
        [JsonPropertyName("added")]
        public List<T>? Added { get; set; }

        [JsonPropertyName("updated")]
        public List<T>? Updated { get; set; }

        [JsonPropertyName("removed")]
        public List<T>? Removed { get; set; }
    }

    // Response DTOs
    public class LoadResponse
    {
        [JsonPropertyName("events")]
        public StoreData<Event>? Events { get; set; }

        [JsonPropertyName("resources")]
        public StoreData<Resource>? Resources { get; set; }
    }

    public class StoreData<T>
    {
        [JsonPropertyName("rows")]
        public List<T> Rows { get; set; } = new List<T>();
    }

    public class SyncResponse
    {
        [JsonPropertyName("requestId")]
        public long? RequestId { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("events")]
        public SyncStoreResponse? Events { get; set; }

        [JsonPropertyName("resources")]
        public SyncStoreResponse? Resources { get; set; }
    }

    public class SyncStoreResponse
    {
        [JsonPropertyName("rows")]
        public List<IdMapping>? Rows { get; set; }
    }

    public class IdMapping
    {
        [JsonPropertyName("$PhantomId")]
        public string? PhantomId { get; set; }

        [JsonPropertyName("id")]
        public object? Id { get; set; }
    }
}
```

The `SyncRequest` class contains the `requestId` and optional `events` and `resources` properties that hold the changes 
for each store. The `StoreChanges<T>` generic class contains lists for added, updated, and removed records. 
The `LoadResponse` and `SyncResponse` classes define the response structure that the Crud Manager expects 
for load and sync requests.

<div class="note">
The <code>$PhantomId</code> is a phantom identifier, a unique, auto-generated client-side value used to identify 
the record. You should not persist phantom identifiers in your database.
</div>

## Create the database context

Create a folder called `Data` in the project directory. In this folder, create a `CalendarContext.cs` file 
containing the following lines of code:

```csharp
using Microsoft.EntityFrameworkCore;
using CalendarApi.Models;

namespace CalendarApi.Data
{
    public class CalendarContext : DbContext
    {
        public CalendarContext(DbContextOptions<CalendarContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Resource> Resources { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.ToTable("resources");
                entity.HasKey(r => r.Id);
            });
        }
    }
}
```

The `CalendarContext` class inherits from `DbContext` and defines `DbSet` properties for each model. 
The `OnModelCreating` method configures the table names and primary keys.

## Configure the .NET backend to use SQLite and seed the local SQLite database with example data

First, install the `Microsoft.EntityFrameworkCore.Sqlite` package:

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Now let's update the `Program.cs` file to configure the .NET backend to use SQLite and create a seeding function that 
populates a local SQLite database with the example JSON data from the `example-data` directory.

Replace the contents of `Program.cs` with the following:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using CalendarApi.Data;
using CalendarApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();

// Configure EF Core to use SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CalendarContext>(options =>
    options.UseSqlite(connectionString)
);

// Add CORS service - Make sure this is before app.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Check if we're running in seed mode
if (args.Contains("--seed"))
{
    await SeedDatabase(app);
    return;
}

// Enable CORS - This MUST be early in the middleware pipeline
app.UseCors("AllowFrontend");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CalendarContext>();
    context.Database.EnsureCreated();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

// Seeding function
static async Task SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CalendarContext>();

    // Drop existing tables and recreate
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
    Console.WriteLine("Database recreated.");

    // Read JSON data from example files
    var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "example-data"));
    
    var eventsJsonPath = Path.Combine(basePath, "events.json");
    var resourcesJsonPath = Path.Combine(basePath, "resources.json");

    Console.WriteLine($"Reading events from: {eventsJsonPath}");
    Console.WriteLine($"Reading resources from: {resourcesJsonPath}");

    var eventsJson = await File.ReadAllTextAsync(eventsJsonPath);
    var resourcesJson = await File.ReadAllTextAsync(resourcesJsonPath);

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    var events = JsonSerializer.Deserialize<List<Event>>(eventsJson, options);
    var resources = JsonSerializer.Deserialize<List<Resource>>(resourcesJson, options);

    if (resources != null && resources.Count > 0)
    {
        await context.Resources.AddRangeAsync(resources);
        await context.SaveChangesAsync();
        Console.WriteLine($"Added {resources.Count} resources.");
    }

    if (events != null && events.Count > 0)
    {
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
        Console.WriteLine($"Added {events.Count} events.");
    }

    Console.WriteLine("Database seeded successfully!");
}
```

This updated `Program.cs` file configures the Entity Framework Core to use SQLite with the connection string 
from `appsettings.json`. It also maps controllers for the API endpoints (which we'll create) and adds CORS configuration 
to allow requests from the frontend running on `http://localhost:5173`.

We can run the `SeedDatabase` function with the `--seed` command line argument to populate the database 
with the example data.

Run the seeding command to create and populate the database:

```shell
dotnet run -- --seed
```

You should see the following output in your terminal:

```
Database seeded successfully!
```

You will also see a `calendar.sqlite3` file created in your project folder. This database is populated with the example 
events and resources data.

## Create an API endpoint to load the Bryntum Calendar data from the database

Create a folder called `Controllers` in the project directory. Create a file called `CalendarController.cs` 
in this folder. First, add the controller class with the `/api/load` endpoint:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApi.Data;
using CalendarApi.Models;

namespace CalendarApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class CalendarController : ControllerBase
    {
        private readonly CalendarContext _context;
        private readonly ILogger<CalendarController> _logger;

        public CalendarController(CalendarContext context, ILogger<CalendarController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("load")]
        public async Task<ActionResult<LoadResponse>> Load()
        {
            try
            {
                var eventsTask = _context.Events.ToListAsync();
                var resourcesTask = _context.Resources.ToListAsync();

                await Task.WhenAll(eventsTask, resourcesTask);

                var response = new LoadResponse
                {
                    Events = new StoreData<Event> { Rows = eventsTask.Result },
                    Resources = new StoreData<Resource> { Rows = resourcesTask.Result }
                };

                _logger.LogInformation("Loaded {EventCount} events and {ResourceCount} resources",
                    eventsTask.Result.Count, resourcesTask.Result.Count);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading data");
                return StatusCode(500, new { success = false, message = "There was an error loading the events and resources data." });
            }
        }
    }
}
```

The `/api/load` endpoint fetches all events and resources from the SQLite database and returns them in the 
[load response structure](https://bryntum.com/products/calendar/docs/guide/Calendar/data/crud_manager#load-response-structure) 
that the Bryntum Calendar Crud Manager expects.

## Create an API endpoint to sync Bryntum Calendar data changes to the database

Below the `Load` method in `CalendarController.cs`, add the following `Sync` method for syncing data changes:

```csharp
[HttpPost("sync")]
public async Task<ActionResult<SyncResponse>> Sync([FromBody] SyncRequest request)
{
    _logger.LogInformation("Sync request received. RequestId: {RequestId}", request.RequestId);
    
    try
    {
        var response = new SyncResponse
        {
            RequestId = request.RequestId,
            Success = true
        };

        if (request.Resources != null)
        {
            var rows = await ApplyResourceChanges(request.Resources);
            if (rows != null && rows.Count > 0)
            {
                response.Resources = new SyncStoreResponse { Rows = rows };
            }
        }

        if (request.Events != null)
        {
            var rows = await ApplyEventChanges(request.Events);
            if (rows != null && rows.Count > 0)
            {
                response.Events = new SyncStoreResponse { Rows = rows };
            }
        }

        return Ok(response);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error syncing data");
        return StatusCode(500, new SyncResponse
        {
            RequestId = request.RequestId,
            Success = false,
            Message = "There was an error syncing the data changes."
        });
    }
}
```

The Bryntum Calendar sends JSON data in POST requests to the `/api/sync` endpoint when there are data changes. 
The request body is parsed to determine which data stores have changed. A Crud Manager sync request includes the changes 
for all the linked data stores in a single request, with a specific [sync request structure](https://bryntum.com/products/calendar/docs/guide/Scheduler/data/crud_manager_in_depth#sync-request-structure). 
We call the `ApplyResourceChanges` and `ApplyEventChanges` helper methods for each data store that has changed. 
Let's create these helper methods next.

### Create the ApplyEventChanges helper method

In `CalendarController.cs`, add the following `ApplyEventChanges` method below the `Sync` method to 
handle event CRUD operations:

```csharp

private async Task<List<IdMapping>?> ApplyEventChanges(StoreChanges<Event> changes)
{
    List<IdMapping>? rows = null;

    if (changes.Added != null && changes.Added.Count > 0)
    {
        rows = new List<IdMapping>();
        foreach (var newEvent in changes.Added)
        {
            // Reset Id to 0 for new events (will be auto-generated)
            newEvent.Id = 0;
            // Ensure Name is not null (required field)
            if (newEvent.Name == null) newEvent.Name = "";

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            rows.Add(new IdMapping
            {
                PhantomId = newEvent.PhantomId,
                Id = newEvent.Id
            });
        }
    }

    if (changes.Updated != null && changes.Updated.Count > 0)
    {
        foreach (var eventUpdate in changes.Updated)
        {
            if (eventUpdate.Id > 0)
            {
                var existingEvent = await _context.Events.FindAsync(eventUpdate.Id);
                if (existingEvent != null)
                {
                    // Update only non-null fields (partial update)
                    if (eventUpdate.Name != null) existingEvent.Name = eventUpdate.Name;
                    
                    // If dates are updated but duration is not explicitly provided, clear duration
                    // so calendar calculates it from the dates
                    bool datesUpdated = false;
                    if (eventUpdate.StartDate.HasValue)
                    {
                        existingEvent.StartDate = eventUpdate.StartDate;
                        datesUpdated = true;
                    }
                    if (eventUpdate.EndDate.HasValue)
                    {
                        existingEvent.EndDate = eventUpdate.EndDate;
                        datesUpdated = true;
                    }
                    
                    if (eventUpdate.AllDay.HasValue) existingEvent.AllDay = eventUpdate.AllDay;
                    if (eventUpdate.ResourceId != null) existingEvent.ResourceId = eventUpdate.ResourceId;
                    if (eventUpdate.EventColor != null) existingEvent.EventColor = eventUpdate.EventColor;
                    if (eventUpdate.ReadOnly.HasValue) existingEvent.ReadOnly = eventUpdate.ReadOnly;
                    if (eventUpdate.TimeZone != null) existingEvent.TimeZone = eventUpdate.TimeZone;
                    if (eventUpdate.Draggable.HasValue) existingEvent.Draggable = eventUpdate.Draggable;
                    if (eventUpdate.Resizable != null) existingEvent.Resizable = eventUpdate.Resizable;
                    
                    // Handle duration: if dates were updated and duration not explicitly provided, clear it
                    if (eventUpdate.Duration.HasValue)
                    {
                        existingEvent.Duration = eventUpdate.Duration;
                    }
                    else if (datesUpdated)
                    {
                        // Dates updated but duration not provided - clear it so calendar calculates from dates
                        existingEvent.Duration = null;
                    }
                    
                    if (eventUpdate.DurationUnit != null) existingEvent.DurationUnit = eventUpdate.DurationUnit;
                    if (eventUpdate.ExceptionDates != null) existingEvent.ExceptionDates = eventUpdate.ExceptionDates;
                    if (eventUpdate.RecurrenceRule != null) existingEvent.RecurrenceRule = eventUpdate.RecurrenceRule;
                    if (eventUpdate.Cls != null) existingEvent.Cls = eventUpdate.Cls;
                    if (eventUpdate.EventStyle != null) existingEvent.EventStyle = eventUpdate.EventStyle;
                    if (eventUpdate.IconCls != null) existingEvent.IconCls = eventUpdate.IconCls;
                    if (eventUpdate.Style != null) existingEvent.Style = eventUpdate.Style;

                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    if (changes.Removed != null && changes.Removed.Count > 0)
    {
        foreach (var eventToRemove in changes.Removed)
        {
            if (eventToRemove.Id > 0)
            {
                var existingEvent = await _context.Events.FindAsync(eventToRemove.Id);
                if (existingEvent != null)
                {
                    _context.Events.Remove(existingEvent);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    return rows;
}
```

This helper method checks whether the change is an `added`, `updated`, or `removed` operation, and then performs 
the appropriate database operation. For added records, we return the phantom ID and the created database ID.

### Create the ApplyResourceChanges helper method

In `CalendarController.cs`, add the `ApplyResourceChanges` method below the `ApplyEventChanges` method to handle 
resource CRUD operations:

```csharp
private async Task<List<IdMapping>?> ApplyResourceChanges(StoreChanges<Resource> changes)
{
    List<IdMapping>? rows = null;

    if (changes.Added != null && changes.Added.Count > 0)
    {
        rows = new List<IdMapping>();
        foreach (var newResource in changes.Added)
        {
            // Generate ID if not provided
            if (string.IsNullOrEmpty(newResource.Id))
            {
                newResource.Id = Guid.NewGuid().ToString();
            }
            // Ensure Name is not null (required field)
            if (newResource.Name == null) newResource.Name = "";

            _context.Resources.Add(newResource);
            await _context.SaveChangesAsync();

            rows.Add(new IdMapping
            {
                PhantomId = newResource.PhantomId,
                Id = newResource.Id
            });
        }
    }

    if (changes.Updated != null && changes.Updated.Count > 0)
    {
        foreach (var resourceUpdate in changes.Updated)
        {
            if (!string.IsNullOrEmpty(resourceUpdate.Id))
            {
                var existingResource = await _context.Resources.FindAsync(resourceUpdate.Id);
                if (existingResource != null)
                {
                    // Update only non-null fields (partial update)
                    if (resourceUpdate.Name != null) existingResource.Name = resourceUpdate.Name;
                    if (resourceUpdate.EventColor != null) existingResource.EventColor = resourceUpdate.EventColor;
                    if (resourceUpdate.ReadOnly.HasValue) existingResource.ReadOnly = resourceUpdate.ReadOnly;

                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    if (changes.Removed != null && changes.Removed.Count > 0)
    {
        foreach (var resourceToRemove in changes.Removed)
        {
            if (!string.IsNullOrEmpty(resourceToRemove.Id))
            {
                var existingResource = await _context.Resources.FindAsync(resourceToRemove.Id);
                if (existingResource != null)
                {
                    _context.Resources.Remove(existingResource);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    return rows;
}
```

Now that the API endpoints have been added, let's test the `/api/load` endpoint.

Run the development server if it's not already running:

```shell
dotnet run
```

Open [http://localhost:1337/api/load](http://localhost:1337/api/load) in your browser. You should see a JSON object of 
the events and resources data from the SQLite database:

```json
{
  "events": {
    "rows": [
      {
        "id": 1,
        "name": "Hackathon 2026",
        ...
```

Now that we've added the API endpoints, let's set up our frontend Bryntum Calendar. 

## Set up the frontend

We'll now configure and add a Bryntum Calendar to the frontend starter project.

### Install the Bryntum Calendar component

First, access the Bryntum private npm registry by following the [guide in our docs](https://bryntum.com/products/calendar/docs/guide/Calendar/quick-start/javascript-npm#access-to-npm-registry). 

Once you've logged in to the registry, install the Bryntum Calendar component:

> TODO: ADD TAB like https://bryntum.com/products/calendar/docs/guide/Calendar/quick-start/javascript-npm#install-component

```shell
npm install @bryntum/calendar
```

```shell
npm install @bryntum/calendar@npm:@bryntum/calendar-trial
```

### Create the calendar configuration

Create a file called `calendarConfig.ts` in the `src` folder and add the following lines of code to it:

```typescript
import { type CalendarConfig } from '@bryntum/calendar';

export const calendarConfig: CalendarConfig = {
    appendTo    : 'app',
    date        : new Date(2026, 6, 20),
    crudManager : {
        loadUrl          : 'http://localhost:1337/api/load',
        autoLoad         : true,
        syncUrl          : 'http://localhost:1337/api/sync',
        autoSync         : true,
        validateResponse : true
    }
};
```

We create a configuration object for the Bryntum Calendar and configure it to attach to the `<div>` element with an 
`id` of `"app"`. The `date` property sets the initial date to display, which is **July 20, 2026**, to match the 
example data.

The Crud Manager uses the [Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API) as a transport system 
and JSON as the encoding format.

We set [`loadUrl`](https://bryntum.com/products/calendar/docs/api/Scheduler/crud/AbstractCrudManagerMixin#config-loadUrl) 
and [`syncUrl`](https://bryntum.com/products/calendar/docs/api/Scheduler/crud/AbstractCrudManagerMixin#config-syncUrl) 
to the .NET API routes we created.

### Create the Bryntum Calendar

Update the `main.ts` file to import and create the Bryntum Calendar:

```typescript
import { Calendar } from '@bryntum/calendar';
import { calendarConfig } from './calendarConfig';
import './style.css';

const calendar = new Calendar(calendarConfig);
```

We import the `Calendar` class from the Bryntum Calendar package and create an instance with our configuration.

### Add styles

Update the `style.css` file in the `src` directory to import the Bryntum Calendar styles and configure the layout:

```css
@import "https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap";
@import "@bryntum/calendar/fontawesome/css/fontawesome.css";
@import "@bryntum/calendar/fontawesome/css/solid.css";
/* Import calendar's structural CSS */
@import "@bryntum/calendar/calendar.css";
/* Import your preferred Bryntum theme */
@import "@bryntum/calendar/svalbard-light.css";

* {
    margin: 0;
}

body,
html {
    font-family: Poppins, "Open Sans", Helvetica, Arial, sans-serif;
}

#app {
    display: flex;
    flex-direction: column;
    height: 100vh;
    font-size: 14px;
}
```

We import the CSS for the Svalbard light theme (one of the four available themes with light and dark variants).

You can also create custom themes. The structural CSS and themes have separate imports. You can read more about styling 
the Calendar in our [docs](https://bryntum.com/products/calendar/docs/guide/Calendar/customization/styling).

## Run the application

To view the completed application, first make sure the backend is running:

```shell
dotnet run
```

Then, in a separate terminal, start the frontend:

```shell
npm run dev
```

Open [http://localhost:5173](http://localhost:5173/) in your browser. You'll see a Bryntum Calendar with 
the example data from the local SQLite database:

![Bryntum Calendar with CRUD functionality](images/bryntum-calendar-crud.webm)

Because the Bryntum Calendar has CRUD functionality, any changes made to it are saved to the SQLite database.

## Next steps

This tutorial covers the basics of using Bryntum Calendar with .NET and SQLite. Take a look at the 
[Bryntum Calendar examples page](https://bryntum.com/products/calendar/examples/) to browse the additional features 
you can add to your Calendar, such as:

- [Dragging events from an external grid](https://bryntum.com/products/calendar/examples/dragfromgrid/)
- [Dragging events between different calendars](https://bryntum.com/products/calendar/examples/drag-between-calendars/)
- [Calendar + Task Board integration](https://bryntum.com/products/calendar/examples/calendar-taskboard/)