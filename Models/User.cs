using WorkoutAPI.Models;
public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public ICollection<Workout> Workouts { get; } = new List<Workout>();
}