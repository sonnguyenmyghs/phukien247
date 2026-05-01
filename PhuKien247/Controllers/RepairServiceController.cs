using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhuKien247.Data;
using PhuKien247.Models;

namespace PhuKien247.Controllers;

public class RepairServiceController : Controller
{
    private readonly ApplicationDbContext _context;

    public RepairServiceController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var services = await _context.RepairServices
            .Where(s => s.IsActive)
            .ToListAsync();

        return View(services);
    }

    [Authorize]
    public async Task<IActionResult> MyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var bookings = await _context.RepairBookings
            .Include(b => b.Service)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return View(bookings);
    }

    [Authorize]
    public async Task<IActionResult> Book(int serviceId)
    {
        var service = await _context.RepairServices.FindAsync(serviceId);
        if (service == null)
        {
            return NotFound();
        }

        ViewBag.Service = service;
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int serviceId, string deviceInfo, string? issueDescription, DateTime bookingDate, string? note)
    {
        var service = await _context.RepairServices.FindAsync(serviceId);
        if (service == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(deviceInfo))
        {
            ModelState.AddModelError("DeviceInfo", "Vui lòng nhập thông tin thiết bị.");
        }

        if (bookingDate < DateTime.Today)
        {
            ModelState.AddModelError("BookingDate", "Ngày hẹn phải từ hôm nay trở đi.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Service = service;
            return View();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var booking = new RepairBooking
        {
            UserId = userId,
            ServiceId = serviceId,
            DeviceInfo = deviceInfo,
            IssueDescription = issueDescription,
            BookingDate = bookingDate,
            Note = note,
            Status = RepairStatus.ChoXacNhan,
            CreatedAt = DateTime.Now
        };

        _context.RepairBookings.Add(booking);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đặt lịch sửa chữa thành công! Chúng tôi sẽ liên hệ xác nhận sớm nhất.";
        return RedirectToAction(nameof(Index));
    }
}
