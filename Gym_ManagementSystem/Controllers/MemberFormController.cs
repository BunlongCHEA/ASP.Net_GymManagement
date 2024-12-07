using Gym_ManagementSystem.Data;
using Gym_ManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Gym_ManagementSystem.Controllers
{
    [Authorize]
    public class MemberFormController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberFormController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var member = await _context.Members.ToListAsync();
            return View(member);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new MemberViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                return Unauthorized();
            }

            //Console.WriteLine(user.Id);

            model.CreatedByUserID = user.Id;
            _context.Add(model);
            await _context.SaveChangesAsync();

            //var errors = ModelState.Values.SelectMany(x => x.Errors);
            //foreach (var error in errors)
            //{
            //    Console.WriteLine(error.ErrorMessage);
            //}

            return RedirectToAction(nameof(Index));
        }

        // GET: MemberForm/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? Id)
        {
            if (null == Id)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(Id);
            if (member != null)
            {
                return View(member);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, MemberViewModel member)
        {
            //Console.WriteLine(Id);
            //Console.WriteLine(member.Id);
            if (Id != member.Id)
            {
                return NotFound();
            }

            // Validate if member_Id is belonging to user
            var memberIdDb = await _context.Members.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != memberIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Update this resource");
            }

            var user = await _userManager.GetUserAsync(User);
            if (null == user)
            {
                return Unauthorized();
            }

            // Retrieve the existing member from the database
            var existingMember = await _context.Members.FindAsync(Id);
            if (null == existingMember)
            {
                return NotFound();
            }

            // Update the fields of the existing member
            existingMember.MemberName = member.MemberName;
            existingMember.Contact = member.Contact;
            existingMember.Email = member.Email;
            existingMember.JoinedMembershipDate = member.JoinedMembershipDate;
            existingMember.CreatedByUserID = user.Id;

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
            var member = await _context.Members.FindAsync(Id);
            if (null == member)
            {
                return NotFound(); //RedirectToAction("Error", "Home", new { message = "Member not found." });
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int Id)
        {
            //Console.WriteLine(Id);
            var member = await _context.Members.FindAsync(Id);
            if (null == member)
            {
                return NotFound(); //RedirectToAction("Error", "Home", new { message = "Member not found." });
            }

            // Validate if member_Id is belonging to user
            var memberIdDb = await _context.Members.FindAsync(Id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != memberIdDb.CreatedByUserID)
            {
                return Forbid("This User is not allowed to Delete this resource");
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        //public IActionResult Index()
        //{
        //    var members = _context.Members.ToList();
        //    return View(members);
        //}

        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Member/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(MemberViewModel member)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the username of the currently logged-in user
        //        var userName = User.Identity.Name;

        //        // Retrieve the user synchronously using UserManager
        //        var user = 

        //        if (user != null)
        //        {
        //            member.CreatedByUserID = user.Id;
        //            _context.Members.Add(member);
        //            _context.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "User not found.");
        //        }
        //    }
        //    return View(member);
        //}

        //[HttpGet]
        //public IActionResult Edit(int memberID)
        //{
        //    var member = _context.Members.Find(memberID);
        //    if (member != null)
        //    {
        //        return View(member);
        //    }
        //    return NotFound();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(MemberViewModel updatedMember)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Members.Update(updatedMember);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "User not found.");
        //    }
        //    return View(updatedMember);
        //}

        //[HttpGet]
        //public IActionResult Delete(int memberID)
        //{
        //    var member = _context.Members.Find(memberID);
        //    if (member != null)
        //    {
        //        return View(member);
        //    }
        //    return NotFound();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirm(int memberID)
        //{
        //    var member = _context.Members.Find(memberID);
        //    if (member != null)
        //    {
        //        _context.Members.Remove(member);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "User not found.");
        //    }

        //    return NotFound();
        //}
    }
}
