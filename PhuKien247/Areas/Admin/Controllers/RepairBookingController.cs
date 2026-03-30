using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhuKien247.Data;
using PhuKien247.Models;

namespace PhuKien247.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RepairBookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public RepairBookingController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bookings = await _context.RepairBookings
            .Include(b => b.User)
            .Include(b => b.Service)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
        return View(bookings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, RepairStatus status)
    {
        var booking = await _context.RepairBookings.FindAsync(id);
        if (booking == null) return NotFound();

        booking.Status = status;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật trạng thái lịch sửa chữa thành công!";
        return RedirectToAction(nameof(Index));
    }
}
