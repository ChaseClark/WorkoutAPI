namespace WorkoutAPI.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
    }
}
