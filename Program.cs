using Microsoft.EntityFrameworkCore;
using WorkoutAPI.DB;
using WorkoutAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add context for postgres db
builder.Services.AddDbContext<WorkoutContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// DB Check
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkoutContext>();
    if (app.Environment.IsDevelopment())
    {
        // always start with clean db for testing
        dbContext.Database.EnsureDeleted();
    }
    dbContext.Database.EnsureCreated();
    if (app.Environment.IsDevelopment())
    {
        //TODO: add seed data
    }
}

// Endpoints

// Workouts
app.MapGet("/workouts", async (WorkoutContext context) =>
    await context.Workouts.ToListAsync());

app.MapGet("/workouts/{id}", async (int id, WorkoutContext context) =>
    await context.Workouts.FindAsync(id) is Workout workout
        ? Results.Ok(workout)
        : Results.NotFound());

app.MapPost("/workouts", async (Workout workout, WorkoutContext context) =>
{
    context.Workouts.Add(workout);
    await context.SaveChangesAsync();
    return Results.Created($"/workouts/{workout.Id}", workout);
});

app.MapPut("/workouts/{id}", async (int id, Workout updatedWorkout, WorkoutContext context) =>
{
    var workout = await context.Workouts.FindAsync(id);
    if (workout is null) return Results.NotFound();

    workout.Name = updatedWorkout.Name;
    workout.Description = updatedWorkout.Description;
    workout.Date = updatedWorkout.Date;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/workouts/{id}", async (int id, WorkoutContext context) =>
{
    var workout = await context.Workouts.FindAsync(id);
    if (workout is null) return Results.NotFound();

    context.Workouts.Remove(workout);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Categories
app.MapGet("/categories", async (WorkoutContext context) =>
    await context.Categories.ToListAsync());

app.MapGet("/categories/{id}", async (int id, WorkoutContext context) =>
    await context.Categories.FindAsync(id) is Category category
        ? Results.Ok(category)
        : Results.NotFound());

app.MapPost("/categories", async (Category category, WorkoutContext context) =>
{
    context.Categories.Add(category);
    await context.SaveChangesAsync();
    return Results.Created($"/categories/{category.Id}", category);
});

app.MapPut("/categories/{id}", async (int id, Category updatedCategory, WorkoutContext context) =>
{
    var category = await context.Categories.FindAsync(id);
    if (category is null) return Results.NotFound();

    category.Name = updatedCategory.Name;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/categories/{id}", async (int id, WorkoutContext context) =>
{
    var category = await context.Categories.FindAsync(id);
    if (category is null) return Results.NotFound();

    context.Categories.Remove(category);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
