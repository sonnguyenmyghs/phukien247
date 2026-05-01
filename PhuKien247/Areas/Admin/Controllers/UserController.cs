using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhuKien247.Data;
using PhuKien247.Models;

namespace PhuKien247.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UserController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var users = await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        var items = new List<UserListItem>();
        foreach (var u in users)
        {
            if (currentUser != null && u.Id == currentUser.Id) continue;

            var roles = await _userManager.GetRolesAsync(u);
            items.Add(new UserListItem
            {
                User = u,
                Roles = roles.ToList(),
                IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.Now
            });
        }

        return View(items);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var orders = await _context.Orders
            .Where(o => o.UserId == id)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var bookings = await _context.RepairBookings
            .Include(b => b.Service)
            .Where(b => b.UserId == id)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        var vm = new UserDetailsViewModel
        {
            User = user,
            Roles = roles.ToList(),
            IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now,
            Orders = orders,
            RepairBookings = bookings
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Id == user.Id)
        {
            TempData["Error"] = "Bạn không thể tự khóa tài khoản của chính mình.";
            return RedirectToAction(nameof(Index));
        }

        var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now;

        if (isLocked)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["Success"] = $"Đã mở khóa tài khoản '{user.FullName}'.";
        }
        else
        {
            // Lock for 100 years (effectively permanent until manually unlocked).
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(100));
            TempData["Success"] = $"Đã khóa tài khoản '{user.FullName}'.";
        }

        return RedirectToAction(nameof(Index));
    }
}

public class UserListItem
{
    public ApplicationUser User { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public bool IsLocked { get; set; }
}

public class UserDetailsViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public bool IsLocked { get; set; }
    public List<Order> Orders { get; set; } = new();
    public List<RepairBooking> RepairBookings { get; set; } = new();
}
