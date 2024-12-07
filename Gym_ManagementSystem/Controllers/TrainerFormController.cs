using Gym_ManagementSystem.Data;
using Gym_ManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gym_ManagementSystem.Controllers
{
    [Authorize]
    public class TrainerFormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainerFormController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var trainer = await _context.Trainers.ToListAsync();
            return View(trainer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TrainerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                return Unauthorized();
            }

            Console.WriteLine(user.Id);

            model.CreatedByUserID = user.Id;
            _context.Add(model);
            await _context.SaveChangesAsync();

            var errors = ModelState.Values.SelectMany(x => x.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (null == Id)
            {
                return BadRequest("No ID: Input the correct ID");
            }

            var trainer = await _context.Trainers.FindAsync(Id);
            if (trainer != null)
            {
                return View(trainer);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, TrainerViewModel model)
        {
            //Console.WriteLine(Id);
            //Console.WriteLine(model.Id);
            if (Id != model.Id)
            {
                return BadRequest("Id and Trainer ID does not match.");
            }

            // Validate if trainer_Id is belonging to user
            var trainerIdDb = await _context.Trainers.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine("trainerIdDb: ", trainerIdDb);
            if (userId != trainerIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Update this resource");
            }

            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                return Unauthorized();
            }

            // Retrieve the existing member from the database
            var existingTrainer = await _context.Trainers.FindAsync(Id);
            if (null == existingTrainer)
            {
                return NotFound();
            }

            // Update the fields of the existing member
            existingTrainer.TrainerName = model.TrainerName;
            existingTrainer.TrainerContact = model.TrainerContact;
            existingTrainer.TrainerEmail = model.TrainerEmail;
            existingTrainer.CreatedByUserID = user.Id;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            //Console.WriteLine(Id);
            var trainer = await _context.Trainers.FindAsync(Id);
            if (null == trainer)
            {
                return NotFound(); //RedirectToAction("Error", "Home", new { message = "Member not found." });
            }
            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int Id)
        {
            //Console.WriteLine(Id);
            var trainer = await _context.Trainers.FindAsync(Id);
            if (null == trainer)
            {
                return NotFound(); //RedirectToAction("Error", "Home", new { message = "Member not found." });
            }

            // Validate if trainer_Id is belonging to user
            var trainerIdDb = await _context.Trainers.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != trainerIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Delete this resource");
            }

            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
