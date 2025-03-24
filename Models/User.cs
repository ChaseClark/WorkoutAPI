using Microsoft.AspNetCore.Identity;
using WorkoutAPI.Models;
public class ApplicationUser : IdentityUser
{
    public ICollection<Workout> Workouts { get; } = new List<Workout>();
    public ICollection<Exercise> Exercises { get; } = new List<Exercise>();
    public ICollection<Category> Categories { get; } = new List<Category>();
}