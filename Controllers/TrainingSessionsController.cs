using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers;

[Authorize]
public class TrainingSessionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrainingSessionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TrainingSessions
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessions = await _context.TrainingSessions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.StartDateTime)
            .ToListAsync();
        
        // Convert UTC to Local for display
        foreach (var session in sessions)
        {
            session.StartDateTime = session.StartDateTime.ToLocalTime();
            session.EndDateTime = session.EndDateTime.ToLocalTime();
        }
        
        return View(sessions);
    }

    // GET: TrainingSessions/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainingSession = await _context.TrainingSessions
            .Include(t => t.ExerciseExecutions)
                .ThenInclude(e => e.ExerciseType)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (trainingSession == null)
        {
            return NotFound();
        }

        // Convert UTC to Local for display
        trainingSession.StartDateTime = trainingSession.StartDateTime.ToLocalTime();
        trainingSession.EndDateTime = trainingSession.EndDateTime.ToLocalTime();

        return View(trainingSession);
    }

    // GET: TrainingSessions/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TrainingSessions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,StartDateTime,EndDateTime,Notes")] TrainingSession trainingSession)
    {
        // Set UserId before validation since it's required but not in the form
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        trainingSession.UserId = userId;
        
        // Remove UserId from ModelState validation since it's set automatically
        ModelState.Remove("UserId");
        
        // Additional validation: EndDateTime must be after StartDateTime
        if (trainingSession.EndDateTime <= trainingSession.StartDateTime)
        {
            ModelState.AddModelError("EndDateTime", "Data i czas zakończenia muszą być późniejsze niż data i czas rozpoczęcia");
        }

        if (ModelState.IsValid)
        {
            // Convert to UTC for PostgreSQL - datetime-local returns Unspecified, so we need to specify it's Local first
            if (trainingSession.StartDateTime.Kind == DateTimeKind.Unspecified)
            {
                trainingSession.StartDateTime = DateTime.SpecifyKind(trainingSession.StartDateTime, DateTimeKind.Local);
            }
            if (trainingSession.EndDateTime.Kind == DateTimeKind.Unspecified)
            {
                trainingSession.EndDateTime = DateTime.SpecifyKind(trainingSession.EndDateTime, DateTimeKind.Local);
            }
            trainingSession.StartDateTime = trainingSession.StartDateTime.ToUniversalTime();
            trainingSession.EndDateTime = trainingSession.EndDateTime.ToUniversalTime();
            _context.Add(trainingSession);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(trainingSession);
    }

    // GET: TrainingSessions/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainingSession = await _context.TrainingSessions
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (trainingSession == null)
        {
            return NotFound();
        }

        // Convert UTC to Local for display
        trainingSession.StartDateTime = trainingSession.StartDateTime.ToLocalTime();
        trainingSession.EndDateTime = trainingSession.EndDateTime.ToLocalTime();

        return View(trainingSession);
    }

    // POST: TrainingSessions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDateTime,EndDateTime,Notes,UserId")] TrainingSession trainingSession)
    {
        if (id != trainingSession.Id)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        // Ensure user can only edit their own sessions
        if (trainingSession.UserId != userId)
        {
            return Forbid();
        }

        // Ensure UserId is set correctly (in case it wasn't in the form)
        trainingSession.UserId = userId;
        ModelState.Remove("UserId");

        // Additional validation: EndDateTime must be after StartDateTime
        if (trainingSession.EndDateTime <= trainingSession.StartDateTime)
        {
            ModelState.AddModelError("EndDateTime", "Data i czas zakończenia muszą być późniejsze niż data i czas rozpoczęcia");
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Convert to UTC for PostgreSQL - datetime-local returns Unspecified, so we need to specify it's Local first
                if (trainingSession.StartDateTime.Kind == DateTimeKind.Unspecified)
                {
                    trainingSession.StartDateTime = DateTime.SpecifyKind(trainingSession.StartDateTime, DateTimeKind.Local);
                }
                if (trainingSession.EndDateTime.Kind == DateTimeKind.Unspecified)
                {
                    trainingSession.EndDateTime = DateTime.SpecifyKind(trainingSession.EndDateTime, DateTimeKind.Local);
                }
                trainingSession.StartDateTime = trainingSession.StartDateTime.ToUniversalTime();
                trainingSession.EndDateTime = trainingSession.EndDateTime.ToUniversalTime();
                _context.Update(trainingSession);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainingSessionExists(trainingSession.Id, userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(trainingSession);
    }

    // GET: TrainingSessions/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainingSession = await _context.TrainingSessions
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (trainingSession == null)
        {
            return NotFound();
        }

        // Convert UTC to Local for display
        trainingSession.StartDateTime = trainingSession.StartDateTime.ToLocalTime();
        trainingSession.EndDateTime = trainingSession.EndDateTime.ToLocalTime();

        return View(trainingSession);
    }

    // POST: TrainingSessions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var trainingSession = await _context.TrainingSessions
            .Include(t => t.ExerciseExecutions)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (trainingSession != null)
        {
            _context.ExerciseExecutions.RemoveRange(trainingSession.ExerciseExecutions);
            _context.TrainingSessions.Remove(trainingSession);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TrainingSessionExists(int id, string userId)
    {
        return _context.TrainingSessions.Any(e => e.Id == id && e.UserId == userId);
    }
}

