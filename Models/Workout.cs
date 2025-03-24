namespace WorkoutAPI.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
        public required string UserId { get; set; }
        public required ApplicationUser User { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; } = new List<WorkoutExercise>(); // Collection navigation containing dependents
    }
}
