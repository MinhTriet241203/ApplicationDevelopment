﻿using FPTBookStore.Data;
using FPTBookStore.Data.Migrations;
using FPTBookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FPTBookStore.Controllers
{
    public class UserRoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRoleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.UserRoles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserRoles'  is null.");
            }

            var UserRoles = (from ur in _context.UserRoles
                             join u in _context.ApplicationUsers
                             on ur.UserId equals u.Id
                             join r in _context.ApplicationRole
                             on ur.RoleId equals r.Id
                             select new UserRoles
                             {
                                 UserId = ur.UserId,
                                 Name = u.Name,
                                 Email = u.Email,
                                 PhoneNumber = u.PhoneNumber,
                                 RoleId = r.Id
                             });

            return View(await UserRoles.ToListAsync());
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.UserRoles == null)
            {
                return NotFound();
            }

            var UserRoles = (from ur in _context.UserRoles
                             join u in _context.ApplicationUsers
                             on ur.UserId equals u.Id
                             join r in _context.ApplicationRole
                             on ur.RoleId equals r.Id
                             where u.Id == id
                             select new UserRoles
                             {
                                 UserId = ur.UserId,
                                 Name = u.Name,
                                 Email = u.Email,
                                 PhoneNumber = u.PhoneNumber,
                                 RoleId = r.Id
                             });

            var userRole = UserRoles.FirstOrDefault(ur => ur.UserId == id);

            if (userRole == null)
            {
                return NotFound();
            }
            ViewData["Roles"] = new SelectList(_context.ApplicationRole, "Id", "Name", userRole.RoleId);
            return View(userRole);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRoles userrole)
        {
            try
            {
                //var userrole = await _context.UserRoles.FirstOrDefaultAsync(u => u.Id == userid);
                var oldRole = await _context.ApplicationRole.FirstOrDefaultAsync(r => r.Id == userrole.RoleId);
                if(userrole != null)
                {
                    var newRole = await _context.ApplicationRole.FirstOrDefaultAsync(r => r.Id == roleid);
                    if(newRole != null)
                    {
                        var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userid);
                        await _userManager.RemoveFromRoleAsync(user, oldRole.Name);
                        await _userManager.AddToRoleAsync(user, newRole.Name);
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {}
            return RedirectToAction(nameof(Index));
        }
    }
}
