using Microsoft.EntityFrameworkCore;
using WorkoutAPI.Models;

namespace WorkoutAPI.DB
{
    public class WorkoutContext : DbContext
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exercise> Exercises { get; set; }



        public WorkoutContext(DbContextOptions<WorkoutContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workout>().ToTable("Workouts");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Exercise>().ToTable("Exercises");


        }
    }
}
