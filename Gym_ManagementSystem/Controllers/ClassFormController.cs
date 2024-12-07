using Gym_ManagementSystem.Data;
using Gym_ManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Gym_ManagementSystem.Controllers
{
    [Authorize]
    public class ClassFormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClassFormController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes.Include( c => c.ClassSchedules ).ToListAsync(); 
            return View(classes);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int Id)
        {
            var classes = await _context.Classes.Include(c => c.ClassSchedules)
                            .FirstOrDefaultAsync(c => c.Id == Id);
            if (null == classes)
            {
                return NotFound();
            }
            return View(classes);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassViewModel classmodel
                                                , List<ClassScheduleViewModel> schedulemodel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                return Unauthorized();
            }

            classmodel.CreatedByUserID = user.Id;
            _context.Add(classmodel);
            await _context.SaveChangesAsync();

            // Add all OpenDate, OpenTime, EndTime list of schedule to ClassScheduleViewModel
            foreach (var schedule in schedulemodel)
            {
                schedule.ClassId = schedule.Id;
                _context.Add(schedule);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
            //return View(classmodel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (null == Id)
            {
                return BadRequest("No ID: Input the correct ID");
            }

            var classes = await _context.Classes.Include(c => c.ClassSchedules)
                .FirstOrDefaultAsync(c => c.Id == Id);

            if(null == classes)
            {
                return NotFound();
            }
            return View(classes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (int Id, ClassViewModel classmodel)
        {
            if (Id != classmodel.Id)
            {
                return BadRequest("Id and Class ID does not match.");
            }

            // Validate if class_Id is belonging to user
            var classIdDb = await _context.Trainers.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != classIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Update this resource");
            }

            try
            {
                // Update CreatedByUserId from ApplicationUser
                var user = await _userManager.GetUserAsync(User);
                if (null == user)
                {
                    return Unauthorized();
                }

                // Fetch existing class
                var existingClass = await _context.Classes
                    .Include(c => c.ClassSchedules) // Include schedules for comparison
                    .FirstOrDefaultAsync(c => c.Id == Id);
                if (null == existingClass)
                {
                    return NotFound();
                }

                // Remove existing schedules first before update new schedules
                var existingSchedule = _context.ClassSchedules.Where(cs => cs.ClassId == classmodel.Id);
                _context.ClassSchedules.RemoveRange(existingSchedule);
                
                // Update existingClass with data from classmodel, can update any other properties here if needed
                existingClass.ClassName = classmodel.ClassName;
                existingClass.CreatedByUserID = user.Id;
                existingClass.ClassSchedules = classmodel.ClassSchedules;

                _context.Update(existingClass);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            var errors = ModelState.Values.SelectMany(x => x.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (null == Id)
            {
                return NotFound();
            }

            var classes = await _context.Classes.Include(c => c.ClassSchedules)
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (null == classes)
            {
                return NotFound();
            }
            return View(classes);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int? Id)
        {
            if (null == Id)
            {
                return NotFound();
            }

            // Validate if class_Id is belonging to user
            var classIdDb = await _context.Trainers.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != classIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Delete this resource");
            }

            var classes = await _context.Classes.Include(c => c.ClassSchedules)
                .FirstOrDefaultAsync(c => c.Id == Id);
            
            if (null != classes)
            {
                // Remove related class schedules first
                _context.ClassSchedules.RemoveRange(classes.ClassSchedules);

                // Then remove class itself
                _context.Classes.Remove(classes);

                // Save change asynchronously
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
