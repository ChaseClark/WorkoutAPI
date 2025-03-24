using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutAPI.Models;

namespace WorkoutAPI.DB
{
    public class WorkoutContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        // public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        public WorkoutContext(DbContextOptions<WorkoutContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // identity

            modelBuilder.Entity<Workout>().ToTable("Workouts");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Exercise>().ToTable("Exercises");
            // modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<WorkoutExercise>().ToTable("WorkoutExercises");

            modelBuilder.Entity<Workout>()
                .HasOne(w => w.User)
                .WithMany(u => u.Workouts)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.User)
                .WithMany(u => u.Exercises)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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
