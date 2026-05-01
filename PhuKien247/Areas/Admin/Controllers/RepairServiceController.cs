using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhuKien247.Data;
using PhuKien247.Models;

namespace PhuKien247.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
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
            .Include(s => s.RepairBookings)
            .OrderByDescending(s => s.Id)
            .ToListAsync();
        return View(services);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairService service)
    {
        if (ModelState.IsValid)
        {
            _context.RepairServices.Add(service);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm dịch vụ sửa chữa thành công!";
            return RedirectToAction(nameof(Index));
        }
        return View(service);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var service = await _context.RepairServices.FindAsync(id);
        if (service == null) return NotFound();
        return View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RepairService service)
    {
        if (id != service.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật dịch vụ thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.RepairServices.AnyAsync(s => s.Id == id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _context.RepairServices
            .Include(s => s.RepairBookings)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (service == null) return NotFound();

        if (service.RepairBookings.Any())
        {
            // Soft delete: deactivate instead of removing because there are existing bookings.
            service.IsActive = false;
            _context.Update(service);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Dịch vụ '{service.Name}' đã được vô hiệu hóa (có {service.RepairBookings.Count} lịch sửa chữa liên quan).";
            return RedirectToAction(nameof(Index));
        }

        _context.RepairServices.Remove(service);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa dịch vụ sửa chữa thành công!";
        return RedirectToAction(nameof(Index));
    }
}
