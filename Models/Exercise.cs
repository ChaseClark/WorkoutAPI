using WorkoutAPI.Models;
public class Exercise
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CategoryID {get; set;}
    public Category Category { get; set; } = null!;
    public required string UserId { get; set; }
    public required ApplicationUser User { get; set; }
    public ICollection<WorkoutExercise> WorkoutExercises { get; } = new List<WorkoutExercise>();

}
