using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
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

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// fix circular refs


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
        SeedData.Initialize(dbContext);
    }
}

// Endpoints

// Users
app.MapGet("/users", async (WorkoutContext context) =>
    await context.Users.ToListAsync());

app.MapGet("/users/{id}", async (int id, WorkoutContext context) =>
    await context.Users.FindAsync(id) is User user
        ? Results.Ok(user)
        : Results.NotFound());


// Workouts
app.MapGet("/workouts", async (WorkoutContext context) =>
    await context.Workouts.Include(w => w.WorkoutExercises).ToListAsync());

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

    workout.Notes = updatedWorkout.Notes;
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


// Exercises
app.MapGet("/exercises", async (WorkoutContext context) =>
    await context.Exercises.Include(e => e.Category).ToListAsync());

// Workout Exercises
app.MapPost("/workouts/{workoutId}/exercises", async (int workoutId, WorkoutExercise workoutExercise, WorkoutContext context) =>
{
    workoutExercise.WorkoutId = workoutId;
    context.WorkoutExercises.Add(workoutExercise);
    await context.SaveChangesAsync();
    return Results.Created($"/workouts/{workoutId}/exercises/{workoutExercise.ExerciseId}", workoutExercise);
});


app.Run();
