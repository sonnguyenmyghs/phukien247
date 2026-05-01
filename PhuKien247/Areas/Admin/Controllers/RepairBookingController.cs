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

    public async Task<IActionResult> Index(string? search, int? status, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.RepairBookings
            .Include(b => b.User)
            .Include(b => b.Service)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b =>
                (b.User.FullName != null && b.User.FullName.Contains(search)) ||
                b.DeviceInfo.Contains(search) ||
                b.Service.Name.Contains(search));
        }

        if (status.HasValue)
        {
            var st = (RepairStatus)status.Value;
            query = query.Where(b => b.Status == st);
        }

        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            query = query.Where(b => b.BookingDate >= from);
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(b => b.BookingDate < to);
        }

        var bookings = await query
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Status = status;
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;

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
