using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;

namespace BeFit.Controllers;

[Authorize]
public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StatisticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Statistics
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Use UTC for comparison with database
        var fourWeeksAgo = DateTime.UtcNow.AddDays(-28);

        // Get all exercise executions from the last 4 weeks for this user
        var exerciseExecutions = await _context.ExerciseExecutions
            .Include(e => e.ExerciseType)
            .Include(e => e.TrainingSession)
            .Where(e => e.TrainingSession != null 
                && e.TrainingSession.UserId == userId 
                && e.TrainingSession.StartDateTime >= fourWeeksAgo)
            .ToListAsync();

        // Group by exercise type and calculate statistics
        var statistics = exerciseExecutions
            .GroupBy(e => e.ExerciseType)
            .Select(g => new StatisticsViewModel
            {
                ExerciseTypeName = g.Key != null ? g.Key.Name : "Nieznany",
                ExerciseTypeId = g.Key != null ? g.Key.Id : 0,
                TimesPerformed = g.Count(),
                TotalRepetitions = g.Sum(e => e.NumberOfSets * e.RepetitionsPerSet),
                AverageWeight = g.Average(e => (double)e.Weight),
                MaxWeight = g.Max(e => (double)e.Weight)
            })
            .OrderBy(s => s.ExerciseTypeName)
            .ToList();

        return View(statistics);
    }
}

public class StatisticsViewModel
{
    public string ExerciseTypeName { get; set; } = string.Empty;
    public int ExerciseTypeId { get; set; }
    public int TimesPerformed { get; set; }
    public int TotalRepetitions { get; set; }
    public double AverageWeight { get; set; }
    public double MaxWeight { get; set; }
}

