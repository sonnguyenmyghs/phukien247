using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhuKien247.Data;
using PhuKien247.Models;

namespace PhuKien247.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalProducts = await _context.Products.CountAsync();
        ViewBag.TotalOrders = await _context.Orders.CountAsync();
        var completedTotals = await _context.Orders
            .Where(o => o.Status == OrderStatus.HoanThanh)
            .Select(o => o.TotalAmount)
            .ToListAsync();
        ViewBag.TotalRevenue = completedTotals.Sum();
        ViewBag.PendingOrders = await _context.Orders
            .Where(o => o.Status == OrderStatus.ChoXacNhan)
            .CountAsync();
        ViewBag.PendingRepairs = await _context.RepairBookings
            .Where(r => r.Status == RepairStatus.ChoXacNhan)
            .CountAsync();

        // Orders by status for doughnut chart
        var ordersByStatus = new Dictionary<string, int>
        {
            { "ChoXacNhan", await _context.Orders.CountAsync(o => o.Status == OrderStatus.ChoXacNhan) },
            { "DaXacNhan", await _context.Orders.CountAsync(o => o.Status == OrderStatus.DaXacNhan) },
            { "DangGiao", await _context.Orders.CountAsync(o => o.Status == OrderStatus.DangGiao) },
            { "HoanThanh", await _context.Orders.CountAsync(o => o.Status == OrderStatus.HoanThanh) },
            { "DaHuy", await _context.Orders.CountAsync(o => o.Status == OrderStatus.DaHuy) }
        };
        ViewBag.OrdersByStatus = ordersByStatus;

        var recentOrders = await _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .ToListAsync();

        return View(recentOrders);
    }
}
