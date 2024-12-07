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
    public class MembershipFormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MembershipFormController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var membership = await _context.Memberships.Include(m => m.Member).ToListAsync();
            return View(membership);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Retrieve members who do not have an associated membership
            var memberNoMembership = await _context.Members
                .Where(m => !_context.Memberships.Any(ms => ms.MemberId == m.Id))
                .Select(m => new {m.Id, m.MemberName})
                .ToListAsync();

            ViewBag.Members = new SelectList(memberNoMembership, "Id", "MemberName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembershipViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                return Unauthorized();
            }

            // Calculate TotalPrice and ExpiredDate (Since we already have all data needed)
            // without user input
            model.TotalPrice = (model.Amount * model.SubscribedMonth) - 
                ((model.Amount * model.SubscribedMonth) * (model.DiscountPercentage / 100));
            model.ExpiredDate = model.RenewalDate.AddMonths(model.SubscribedMonth);
            model.CreatedByUserID = user.Id;

            _context.Add(model);
            await _context.SaveChangesAsync();

            ViewBag.Members = new SelectList(_context.Members, "Id", "MemberName", model.MemberId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (null == Id)
            {
                return NotFound();
            }

            var membership = await _context.Memberships.FindAsync(Id);
            if (null == membership)
            {
                return NotFound();
            }

            // Find MemberName with Id, and display to View
            var memberNameView = await _context.Members.FindAsync(membership.MemberId);
            ViewBag.MemberName = memberNameView?.MemberName ?? "Unknown";
            return View(membership);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? Id, MembershipViewModel model)
        {
            //Console.WriteLine("Id", Id);
            if (Id != model.Id || null == Id)
            {
                return BadRequest("Id and Workout ID does not match.");
            }

            // Validate if membership_Id is belonging to user
            var membershipIdDb = await _context.Memberships.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != membershipIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Update this resource");
            }

            // Get User Id to pass database
            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                return Unauthorized();
            }

            // Fetch existing class, ensure the membership is being tracked correctly
            var existingMembership = await _context.Memberships
                .FirstOrDefaultAsync(w => w.Id == Id);
            if (null == existingMembership)
            {
                return NotFound();
            }

            // Map changes to existingMembership, and re-calculate TotalPrice and ExpiredDate
            // (Since we already have all data needed) without user input
            //existingMembership.MemberId = model.MemberId;
            existingMembership.SubscribedMonth = model.SubscribedMonth;
            existingMembership.Amount = model.Amount;
            existingMembership.DiscountPercentage = model.DiscountPercentage;
            existingMembership.RenewalDate = model.RenewalDate;
            existingMembership.TotalPrice = (model.Amount * model.SubscribedMonth) -
                ((model.Amount * model.SubscribedMonth) * (model.DiscountPercentage / 100));
            existingMembership.ExpiredDate = model.RenewalDate.AddMonths(model.SubscribedMonth);
            existingMembership.CreatedByUserID = user.Id;

            _context.Update(existingMembership);
            await _context.SaveChangesAsync();

            //ViewBag.Members = new SelectList(_context.Members, "Id", "MemberName", model.MemberId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (null == Id)
            {
                return NotFound();
            }

            var membership = await _context.Memberships
                                .Include(w => w.Member)
                                .FirstOrDefaultAsync(w => w.Id == Id);

            if (null == membership)
            {
                return NotFound();
            }

            return View(membership);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int? Id)
        {
            // Validate if membership_Id is belonging to user
            var membershipIdDb = await _context.Memberships.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != membershipIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Delete this resource");
            }

            var membership = await _context.Memberships.FindAsync(Id);
            if (null != membership)
            {
                _context.Memberships.Remove(membership);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        
    }
}
