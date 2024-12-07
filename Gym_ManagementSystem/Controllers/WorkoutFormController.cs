using Gym_ManagementSystem.Data;
using Gym_ManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gym_ManagementSystem.Controllers
{
    [Authorize]
    public class WorkoutFormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkoutFormController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var workouts = await _context.Workouts
                .Include(w => w.Class)
                .Include(w => w.Trainer)
                .Include(w => w.Member)
                .ToListAsync();

            return View(workouts);
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if(null == Id)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .Include(w => w.Class)
                .ThenInclude(c => c.ClassSchedules)
                .Include(w => w.Member)
                .Include(w => w.Trainer)
                .FirstOrDefaultAsync(m => m.Id == Id);
            
            if (null == workout)
            {
                return NotFound();
            }

            return View(workout);
        }

        // GET: WorkoutForm/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // View Data dictionary, pass data from Controller to View with dropdown display
            // ClassName, TrainerName, MemberName, instead of Id
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName");
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "TrainerName");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "MemberName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                return Unauthorized();
            }

            model.CreatedByUserID = user.Id;
            _context.Add(model);
            await _context.SaveChangesAsync();

            // View Data dictionary, pass data from Controller to View with dropdown display
            // ClassName, TrainerName, MemberName, instead of Id. While model.ClassId, other will pre-selected
            // For display the previous selected option
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName", model.ClassId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "TrainerName", model.TrainerId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "MemberName", model.MemberId);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (null == Id)
            {
                return NotFound();
            }

            var workout = await _context.Workouts.FindAsync(Id);
            if (null == workout)
            {
                return NotFound();
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName", workout.ClassId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "TrainerName", workout.TrainerId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "MemberName", workout.MemberId);
            
            return View(workout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? Id, WorkoutViewModel model)
        {
            //Console.WriteLine("Id", Id);
            if (Id != model.Id || null == Id)
            {
                return BadRequest("Id and Workout ID does not match.");
            }

            // Validate if workout_Id is belonging to user
            var workoutIdDb = await _context.Workouts.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != workoutIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Update this resource");
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (null == user)
                {
                    return Unauthorized();
                }

                // Fetch existing class, ensure the workout is being tracked correctly
                var existingWorkout = await _context.Workouts
                    .FirstOrDefaultAsync(w => w.Id == Id);
                if (null == existingWorkout)
                {
                    return NotFound();
                }

                // Map changes to existingWorkout
                existingWorkout.CreatedByUserID = user.Id;
                existingWorkout.ClassId = model.ClassId;
                existingWorkout.TrainerId = model.TrainerId;
                existingWorkout.MemberId = model.MemberId;

                _context.Update(existingWorkout);
                // Attach the existing entity back into the context to track changes
                //_context.Workouts.Attach(existingWorkout);
                //_context.Entry(existingWorkout).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "ClassName", model.ClassId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "TrainerName", model.TrainerId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "MemberName", model.MemberId);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (null == Id)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .Include(w => w.Class)
                .Include(w => w.Trainer)
                .Include(w => w.Member)
                .FirstOrDefaultAsync(w => w.Id == Id);

            if (null == workout)
            {
                return NotFound();
            }

            return View(workout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm (int? Id)
        {
            // Validate if workout_Id is belonging to user
            var workoutIdDb = await _context.Workouts.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != workoutIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Delete this resource");
            }

            var workout = await _context.Workouts.FindAsync(Id);
            if (null != workout)
            {
                _context.Workouts.Remove(workout);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
