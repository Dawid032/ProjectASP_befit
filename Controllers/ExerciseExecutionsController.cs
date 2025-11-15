using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;

namespace BeFit.Controllers;

[Authorize]
public class ExerciseExecutionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExerciseExecutionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ExerciseExecutions
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var executions = await _context.ExerciseExecutions
            .Include(e => e.ExerciseType)
            .Include(e => e.TrainingSession)
            .Where(e => e.TrainingSession != null && e.TrainingSession.UserId == userId)
            .OrderByDescending(e => e.TrainingSession!.StartDateTime)
            .ToListAsync();
        
        // Convert UTC to Local for display
        foreach (var execution in executions)
        {
            if (execution.TrainingSession != null)
            {
                execution.TrainingSession.StartDateTime = execution.TrainingSession.StartDateTime.ToLocalTime();
                execution.TrainingSession.EndDateTime = execution.TrainingSession.EndDateTime.ToLocalTime();
            }
        }
        
        return View(executions);
    }

    // GET: ExerciseExecutions/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exerciseExecution = await _context.ExerciseExecutions
            .Include(e => e.ExerciseType)
            .Include(e => e.TrainingSession)
            .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession != null && m.TrainingSession.UserId == userId);

        if (exerciseExecution == null)
        {
            return NotFound();
        }

        // Convert UTC to Local for display
        if (exerciseExecution.TrainingSession != null)
        {
            exerciseExecution.TrainingSession.StartDateTime = exerciseExecution.TrainingSession.StartDateTime.ToLocalTime();
            exerciseExecution.TrainingSession.EndDateTime = exerciseExecution.TrainingSession.EndDateTime.ToLocalTime();
        }

        return View(exerciseExecution);
    }

    // GET: ExerciseExecutions/Create
    public async Task<IActionResult> Create()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessions = await _context.TrainingSessions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.StartDateTime)
            .ToListAsync();
        
        // Convert UTC to Local for display in dropdown
        foreach (var session in sessions)
        {
            session.StartDateTime = session.StartDateTime.ToLocalTime();
            session.EndDateTime = session.EndDateTime.ToLocalTime();
        }
        
        ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name");
        ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Name");
        return View();
    }

    // POST: ExerciseExecutions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TrainingSessionId,ExerciseTypeId,Weight,NumberOfSets,RepetitionsPerSet,Notes")] ExerciseExecution exerciseExecution)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Verify that the training session belongs to the current user
        var trainingSession = await _context.TrainingSessions
            .FirstOrDefaultAsync(t => t.Id == exerciseExecution.TrainingSessionId && t.UserId == userId);
        
        if (trainingSession == null)
        {
            ModelState.AddModelError("TrainingSessionId", "Nieprawidłowa sesja treningowa");
        }

        if (ModelState.IsValid)
        {
            _context.Add(exerciseExecution);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        var sessions = await _context.TrainingSessions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.StartDateTime)
            .ToListAsync();
        
        // Convert UTC to Local for display in dropdown
        foreach (var session in sessions)
        {
            session.StartDateTime = session.StartDateTime.ToLocalTime();
            session.EndDateTime = session.EndDateTime.ToLocalTime();
        }
        
        ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exerciseExecution.ExerciseTypeId);
        ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Name", exerciseExecution.TrainingSessionId);
        return View(exerciseExecution);
    }

    // GET: ExerciseExecutions/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exerciseExecution = await _context.ExerciseExecutions
            .Include(e => e.TrainingSession)
            .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession != null && m.TrainingSession.UserId == userId);

        if (exerciseExecution == null)
        {
            return NotFound();
        }

        var sessions = await _context.TrainingSessions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.StartDateTime)
            .ToListAsync();
        
        // Convert UTC to Local for display in dropdown
        foreach (var session in sessions)
        {
            session.StartDateTime = session.StartDateTime.ToLocalTime();
            session.EndDateTime = session.EndDateTime.ToLocalTime();
        }

        ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exerciseExecution.ExerciseTypeId);
        ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "StartDateTime", exerciseExecution.TrainingSessionId);
        return View(exerciseExecution);
    }

    // POST: ExerciseExecutions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TrainingSessionId,ExerciseTypeId,Weight,NumberOfSets,RepetitionsPerSet,Notes")] ExerciseExecution exerciseExecution)
    {
        if (id != exerciseExecution.Id)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Verify that the training session belongs to the current user
        var trainingSession = await _context.TrainingSessions
            .FirstOrDefaultAsync(t => t.Id == exerciseExecution.TrainingSessionId && t.UserId == userId);
        
        if (trainingSession == null)
        {
            ModelState.AddModelError("TrainingSessionId", "Nieprawidłowa sesja treningowa");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(exerciseExecution);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (userId == null || !ExerciseExecutionExists(exerciseExecution.Id, userId))
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
        
        var sessions = await _context.TrainingSessions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.StartDateTime)
            .ToListAsync();
        
        // Convert UTC to Local for display in dropdown
        foreach (var session in sessions)
        {
            session.StartDateTime = session.StartDateTime.ToLocalTime();
            session.EndDateTime = session.EndDateTime.ToLocalTime();
        }
        
        ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseTypes, "Id", "Name", exerciseExecution.ExerciseTypeId);
        ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Name", exerciseExecution.TrainingSessionId);
        return View(exerciseExecution);
    }

    // GET: ExerciseExecutions/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exerciseExecution = await _context.ExerciseExecutions
            .Include(e => e.ExerciseType)
            .Include(e => e.TrainingSession)
            .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession != null && m.TrainingSession.UserId == userId);

        if (exerciseExecution == null)
        {
            return NotFound();
        }

        // Convert UTC to Local for display
        if (exerciseExecution.TrainingSession != null)
        {
            exerciseExecution.TrainingSession.StartDateTime = exerciseExecution.TrainingSession.StartDateTime.ToLocalTime();
            exerciseExecution.TrainingSession.EndDateTime = exerciseExecution.TrainingSession.EndDateTime.ToLocalTime();
        }

        return View(exerciseExecution);
    }

    // POST: ExerciseExecutions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exerciseExecution = await _context.ExerciseExecutions
            .Include(e => e.TrainingSession)
            .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession != null && m.TrainingSession.UserId == userId);

        if (exerciseExecution != null)
        {
            _context.ExerciseExecutions.Remove(exerciseExecution);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ExerciseExecutionExists(int id, string userId)
    {
        return _context.ExerciseExecutions
            .Any(e => e.Id == id && e.TrainingSession != null && e.TrainingSession.UserId == userId);
    }
}

