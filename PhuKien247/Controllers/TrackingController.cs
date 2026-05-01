using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhuKien247.Data;
using PhuKien247.Models;

namespace PhuKien247.Controllers;

public class TrackingController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrackingController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(int orderId, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            ViewBag.Error = "Vui lòng nhập số điện thoại.";
            ViewBag.OrderId = orderId;
            ViewBag.PhoneNumber = phoneNumber;
            return View();
        }

        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.PhoneNumber == phoneNumber);

        if (order == null)
        {
            ViewBag.Error = "Không tìm thấy đơn hàng. Vui lòng kiểm tra lại mã đơn hàng và số điện thoại.";
            ViewBag.OrderId = orderId;
            ViewBag.PhoneNumber = phoneNumber;
            return View();
        }

        ViewBag.Order = order;
        ViewBag.OrderId = orderId;
        ViewBag.PhoneNumber = phoneNumber;
        return View();
    }

    [HttpGet]
    public IActionResult RepairLookup()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("RepairLookup")]
    public async Task<IActionResult> RepairLookupPost(int bookingId, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            ViewBag.Error = "Vui lòng nhập số điện thoại.";
            ViewBag.BookingId = bookingId;
            ViewBag.PhoneNumber = phoneNumber;
            return View("RepairLookup");
        }

        var booking = await _context.RepairBookings
            .Include(b => b.User)
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.User.PhoneNumber == phoneNumber);

        if (booking == null)
        {
            ViewBag.Error = "Không tìm thấy lịch sửa chữa. Vui lòng kiểm tra lại mã lịch hẹn và số điện thoại.";
            ViewBag.BookingId = bookingId;
            ViewBag.PhoneNumber = phoneNumber;
            return View("RepairLookup");
        }

        ViewBag.Booking = booking;
        ViewBag.BookingId = bookingId;
        ViewBag.PhoneNumber = phoneNumber;
        return View("RepairLookup");
    }
}
