using System.Text.Json.Serialization;

namespace WorkoutAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Exercise> Exercises { get; } = new List<Exercise>();

    }
}
