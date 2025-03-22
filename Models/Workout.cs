namespace WorkoutAPI.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; } = new List<WorkoutExercise>(); // Collection navigation containing dependents
    }
}
