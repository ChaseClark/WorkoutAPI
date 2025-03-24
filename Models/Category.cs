using System.Text.Json.Serialization;

namespace WorkoutAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string UserId { get; set; }
        public required ApplicationUser User { get; set; }
        public ICollection<Exercise> Exercises { get; } = new List<Exercise>();

    }
}
