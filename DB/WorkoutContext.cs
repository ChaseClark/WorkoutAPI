using Microsoft.EntityFrameworkCore;
using WorkoutAPI.Models;

namespace WorkoutAPI.DB
{
    public class WorkoutContext : DbContext
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        public WorkoutContext(DbContextOptions<WorkoutContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workout>().ToTable("Workouts");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Exercise>().ToTable("Exercises");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<WorkoutExercise>().ToTable("WorkoutExercises");

            // Configure composite key for WorkoutExercise
            modelBuilder.Entity<WorkoutExercise>()
                .HasKey(we => new { we.WorkoutId, we.ExerciseId });

            // Optional / But we can declare here for clarity
            // EF core can infer these
            modelBuilder.Entity<Workout>()
                .HasMany(w => w.WorkoutExercises)
                .WithOne(we => we.Workout)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.WorkoutExercises)
                .WithOne(we => we.Exercise)
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
