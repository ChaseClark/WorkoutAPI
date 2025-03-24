// using Microsoft.EntityFrameworkCore;
// using WorkoutAPI.DB;
// using WorkoutAPI.Models;
// using System;
// using System.Linq;

// public static class SeedData
// {
//     public static void Initialize(WorkoutContext context)
//     {
//         // Seed Users
//         var users = new ApplicationUser[]
//         {
//             new ApplicationUser { Email = "test@test.com" },
//         };


//         // Seed Categories
//         var categories = new Category[]
//         {
//             new Category { Name = "Push" },
//             new Category { Name = "Pull" },
//             new Category { Name = "Legs" },
//         };

//         context.Categories.AddRange(categories);
//         context.SaveChanges();

//         // Seed Exercises
//         var exercises = new Exercise[]
//         {
//             new Exercise { Name = "Pushups",CategoryID=1,Category=categories.Where((x) => x.Id == 1).First() },
//             new Exercise { Name = "Pullups",CategoryID=2,Category=categories.Where((x) => x.Id == 2).First() },
//             new Exercise { Name = "Squats",CategoryID=3,Category=categories.Where((x) => x.Id == 3).First() },
//         };

//         context.Exercises.AddRange(exercises);
//         context.SaveChanges();

//         // Seed Workouts
//         var workouts = new Workout[]
//         {
//             new Workout
//             {
//                 Notes = "bodyweight pushups",
//                 Date = DateTime.Now.ToUniversalTime().AddDays(2),
//                 User=users.Where((x) => x.Id == 0).First(),

//             },
//             new Workout
//             {
//                 Notes = "Full body workout with weights",
//                 Date = DateTime.Now.ToUniversalTime().AddDays(-1),
//                 User=users.Where((x) => x.Id == 0).First(),
//             },
//             new Workout
//             {
//                 Notes = "1-hour yoga session for flexibility",
//                 Date = DateTime.Now.ToUniversalTime(),
//                 User=users.Where((x) => x.Id == 0).First(),
//             },
//         };

//         context.Workouts.AddRange(workouts);
//         context.SaveChanges();

//         // Seed WorkoutExercises
//         var workoutexercises = new WorkoutExercise[]
//         {
//             new WorkoutExercise
//             {
//                 Sets = 3,
//                 Reps = 10,
//                 ExerciseId = 1,
//                 Exercise = exercises.Where(e => e.Id == 1).First(),
//                 WorkoutId = 1,
//                 Workout = workouts.Where(w => w.Id == 1).First(),
//             },
//             new WorkoutExercise
//             {
//                 Sets = 3,
//                 Reps = 10,
//                 ExerciseId = 2,
//                 Exercise = exercises.Where(e => e.Id == 2).First(),
//                 WorkoutId = 1,
//                 Workout = workouts.Where(w => w.Id == 1).First(),
//             },
//             new WorkoutExercise
//             {
//                 Sets = 3,
//                 Reps = 10,
//                 ExerciseId = 3,
//                 Exercise = exercises.Where(e => e.Id == 3).First(),
//                 WorkoutId = 2,
//                 Workout = workouts.Where(w => w.Id == 2).First(),
//             },
//         };
//         context.WorkoutExercises.AddRange(workoutexercises);
//         context.SaveChanges();
//     }
// }