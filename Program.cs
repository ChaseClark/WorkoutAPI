using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkoutAPI.DB;
using WorkoutAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; // Enforce unique emails
    // options.User.AllowedUserNameCharacters = null; // Allow all characters in username
    options.Stores.MaxLengthForKeys = 128; // For database key length
})
    .AddEntityFrameworkStores<WorkoutContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };
});

// Add authorization
builder.Services.AddAuthorization();


// add context for postgres db
builder.Services.AddDbContext<WorkoutContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// identity middleware
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAngular");

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
        // dbContext.Database.EnsureDeleted();
    }
    dbContext.Database.EnsureCreated();
    // if (app.Environment.IsDevelopment())
    // {
    //     SeedData.Initialize(dbContext);
    // }
}

// Endpoints

// Users
// app.MapGet("/users", async (WorkoutContext context) =>
//     await context.Users.ToListAsync());

// app.MapGet("/users/{id}", async (int id, WorkoutContext context) =>
//     await context.Users.FindAsync(id) is ApplicationUser user
//         ? Results.Ok(user)
//         : Results.NotFound());


// Workouts
app.MapGet("/workouts", [Authorize] async (WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
    {
        var user = await userManager.GetUserAsync(httpContext.User);
        return await context.Workouts
            .Where(w => w.UserId == user!.Id)
            .Include(w => w.WorkoutExercises)
            .ToListAsync();
    });

app.MapPost("/workouts", [Authorize] async (
    HttpContext httpContext, // Add this parameter
    Workout workout,
    WorkoutContext context,
    UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(httpContext.User);
    workout.UserId = user!.Id;

    context.Workouts.Add(workout);
    await context.SaveChangesAsync();
    return Results.Created($"/workouts/{workout.Id}", workout);
});

app.MapPut("/workouts/{id}", [Authorize] async (int id, Workout updatedWorkout, WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(httpContext.User);
    var workout = await context.Workouts.Where(w => w.User.Id == user!.Id && w.Id == id).FirstAsync();
    if (workout is null) return Results.NotFound();

    workout.Notes = updatedWorkout.Notes;
    workout.Date = updatedWorkout.Date;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/workouts/{id}", [Authorize] async (int id, WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(httpContext.User);
    var workout = await context.Workouts.Where(w => w.User.Id == user!.Id && w.Id == id).FirstAsync();
    if (workout is null) return Results.NotFound();

    context.Workouts.Remove(workout);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Categories
app.MapGet("/categories", [Authorize] async (WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(httpContext.User);
    await context.Categories.Where(c => c.UserId == user!.Id).Include(c => c.Exercises).ToListAsync();
});

app.MapPost("/categories", [Authorize] async (Category category, WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(httpContext.User);
    category.UserId = user!.Id;
    context.Categories.Add(category);
    await context.SaveChangesAsync();
    return Results.Created($"/categories/{category.Id}", category);
});

app.MapPut("/categories/{id}", [Authorize] async (int id, Category updatedCategory, WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(httpContext.User);
    var category = await context.Categories.Where(c => c.User.Id == user!.Id && c.Id == id).FirstAsync();
    if (category is null) return Results.NotFound();

    category.Name = updatedCategory.Name;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/categories/{id}", [Authorize] async (int id, WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(httpContext.User);
    var category = await context.Categories.Where(c => c.User.Id == user!.Id && c.Id == id).FirstAsync();
    if (category is null) return Results.NotFound();

    context.Categories.Remove(category);
    await context.SaveChangesAsync();
    return Results.NoContent();
});


// Exercises
app.MapGet("/exercises", [Authorize] async (WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(httpContext.User);
    return await context.Exercises
            .Where(e => e.UserId == user!.Id)
            .Include(e => e.WorkoutExercises)
            .ToListAsync();
});
// Workout Exercises
app.MapPost("/workouts/{workoutId}/exercises", [Authorize] async (int workoutId, WorkoutExercise workoutExercise, WorkoutContext context, HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
{
    workoutExercise.WorkoutId = workoutId;
    context.WorkoutExercises.Add(workoutExercise);
    await context.SaveChangesAsync();
    return Results.Created($"/workouts/{workoutId}/exercises/{workoutExercise.ExerciseId}", workoutExercise);
});

// Auth Endpoints
app.MapPost("/register", async (RegisterDto registerDto, UserManager<ApplicationUser> userManager) =>
{
    if (await userManager.FindByEmailAsync(registerDto.Email) != null)
    {
        return Results.Conflict("Email already exists.");
    }
    var user = new ApplicationUser { UserName = registerDto.Email, Email = registerDto.Email };
    var result = await userManager.CreateAsync(user, registerDto.Password);

    if (!result.Succeeded)
        return Results.BadRequest(result.Errors);

    return Results.Created($"/users/{user.Id}", new { user.Id, user.Email });
});

app.MapPost("/login", async (LoginDto loginDto, UserManager<ApplicationUser> userManager, IConfiguration config) =>
{
    var user = await userManager.FindByEmailAsync(loginDto.Email);
    if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
        return Results.Unauthorized();

    var authClaims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Email, user.Email!),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

    var jwtSettings = config.GetSection("JwtSettings");
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
    var token = new JwtSecurityToken(
        issuer: jwtSettings["ValidIssuer"],
        audience: jwtSettings["ValidAudience"],
        claims: authClaims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return Results.Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token),
        expiration = token.ValidTo
    });
});

app.Run();


public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);