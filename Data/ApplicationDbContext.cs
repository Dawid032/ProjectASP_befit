using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BeFit.Models;

namespace BeFit.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ExerciseType> ExerciseTypes { get; set; }
    public DbSet<TrainingSession> TrainingSessions { get; set; }
    public DbSet<ExerciseExecution> ExerciseExecutions { get; set; }
}
