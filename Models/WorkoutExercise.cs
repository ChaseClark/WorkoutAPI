using WorkoutAPI.Models;
public class WorkoutExercise
{
    public int Id { get; set; }

    public int Sets { get; set; }

    public int Reps { get; set; }

    public decimal Weight { get; set; }

    public int WorkoutId { get; set; } // Required foreign key property
    public required Workout Workout { get; set; } // Required reference navigation to principal
    public int ExerciseId { get; set; }
    public required Exercise Exercise { get; set; }

}