using Microsoft.AspNetCore.Identity;
using PhuKien247.Models;

namespace PhuKien247.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Tạo roles
        string[] roles = { "Admin", "Customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Tạo Admin account
        if (await userManager.FindByEmailAsync("admin@phukien247.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@phukien247.com",
                Email = "admin@phukien247.com",
                FullName = "Quản trị viên",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed Categories
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Ốp lưng", Description = "Ốp lưng điện thoại các loại" },
                new Category { Name = "Cường lực", Description = "Kính cường lực bảo vệ màn hình" },
                new Category { Name = "Sạc & Cáp", Description = "Bộ sạc và cáp kết nối" },
                new Category { Name = "Tai nghe", Description = "Tai nghe có dây và không dây" },
                new Category { Name = "Pin dự phòng", Description = "Sạc dự phòng các dung lượng" },
                new Category { Name = "Giá đỡ", Description = "Giá đỡ điện thoại, kẹp xe" }
            );
            await context.SaveChangesAsync();
        }

        // Seed Brands
        if (!context.Brands.Any())
        {
            context.Brands.AddRange(
                new Brand { Name = "Apple" },
                new Brand { Name = "Samsung" },
                new Brand { Name = "Xiaomi" },
                new Brand { Name = "Anker" },
                new Brand { Name = "Baseus" },
                new Brand { Name = "Ugreen" }
            );
            await context.SaveChangesAsync();
        }

        // Seed Products
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Ốp lưng iPhone 15 Pro Max trong suốt", Price = 150000, SalePrice = 99000, Stock = 50, CategoryId = 1, BrandId = 1, Description = "Ốp lưng silicon trong suốt, chống sốc, bảo vệ camera" },
                new Product { Name = "Ốp lưng Samsung S24 Ultra chống sốc", Price = 200000, SalePrice = 149000, Stock = 40, CategoryId = 1, BrandId = 2, Description = "Ốp lưng chống sốc 4 góc, chất liệu TPU cao cấp" },
                new Product { Name = "Kính cường lực iPhone 15 Pro Max", Price = 100000, SalePrice = 69000, Stock = 100, CategoryId = 2, BrandId = 1, Description = "Kính cường lực 9H, full màn, chống bám vân tay" },
                new Product { Name = "Kính cường lực Samsung S24 Ultra UV", Price = 250000, Stock = 30, CategoryId = 2, BrandId = 2, Description = "Kính cường lực UV full keo, độ cứng 9H+" },
                new Product { Name = "Cáp sạc Type-C Anker 1.8m", Price = 180000, SalePrice = 139000, Stock = 80, CategoryId = 3, BrandId = 4, Description = "Cáp sạc nhanh 60W, dây dù chống đứt" },
                new Product { Name = "Bộ sạc nhanh 20W Baseus", Price = 250000, SalePrice = 189000, Stock = 60, CategoryId = 3, BrandId = 5, Description = "Sạc nhanh PD 20W cho iPhone và Android" },
                new Product { Name = "Tai nghe Bluetooth Xiaomi Buds 4", Price = 900000, SalePrice = 690000, Stock = 25, CategoryId = 4, BrandId = 3, Description = "Tai nghe true wireless, chống ồn chủ động ANC" },
                new Product { Name = "Pin dự phòng Anker 10000mAh", Price = 500000, SalePrice = 399000, Stock = 35, CategoryId = 5, BrandId = 4, Description = "Pin dự phòng nhỏ gọn, sạc nhanh 22.5W" },
                new Product { Name = "Pin dự phòng Baseus 20000mAh", Price = 700000, SalePrice = 549000, Stock = 20, CategoryId = 5, BrandId = 5, Description = "Pin dự phòng dung lượng lớn, 2 cổng USB-C" },
                new Product { Name = "Giá đỡ điện thoại Ugreen cho xe hơi", Price = 200000, SalePrice = 159000, Stock = 45, CategoryId = 6, BrandId = 6, Description = "Giá kẹp điện thoại gắn cửa gió xe hơi, xoay 360 độ" },
                new Product { Name = "Cáp Lightning Ugreen MFi 1m", Price = 150000, Stock = 70, CategoryId = 3, BrandId = 6, Description = "Cáp Lightning chứng nhận MFi, sạc nhanh cho iPhone" },
                new Product { Name = "Tai nghe có dây Samsung Type-C", Price = 250000, Stock = 40, CategoryId = 4, BrandId = 2, Description = "Tai nghe AKG cổng Type-C, âm thanh Hi-Res" }
            );
            await context.SaveChangesAsync();
        }

        // Seed Repair Services
        if (!context.RepairServices.Any())
        {
            context.RepairServices.AddRange(
                new RepairService { Name = "Thay màn hình iPhone", Price = 1500000, EstimatedTime = "1-2 giờ", Description = "Thay thế màn hình chính hãng cho các dòng iPhone" },
                new RepairService { Name = "Thay màn hình Samsung", Price = 1200000, EstimatedTime = "1-2 giờ", Description = "Thay thế màn hình AMOLED cho các dòng Samsung Galaxy" },
                new RepairService { Name = "Thay pin iPhone", Price = 500000, EstimatedTime = "30 phút", Description = "Thay pin dung lượng chuẩn, bảo hành 6 tháng" },
                new RepairService { Name = "Thay pin Samsung", Price = 400000, EstimatedTime = "30 phút", Description = "Thay pin chính hãng cho Samsung, bảo hành 6 tháng" },
                new RepairService { Name = "Sửa lỗi phần mềm", Price = 200000, EstimatedTime = "30-60 phút", Description = "Cài đặt lại, khắc phục lỗi treo, đơ máy" },
                new RepairService { Name = "Thay camera sau", Price = 800000, EstimatedTime = "1 giờ", Description = "Thay thế cụm camera sau cho iPhone và Samsung" },
                new RepairService { Name = "Thay loa ngoài", Price = 300000, EstimatedTime = "30 phút", Description = "Thay thế loa ngoài bị rè, nhỏ tiếng" },
                new RepairService { Name = "Vệ sinh, bảo dưỡng máy", Price = 100000, EstimatedTime = "30 phút", Description = "Vệ sinh bên trong, tra keo chống nước, kiểm tra tổng thể" }
            );
            await context.SaveChangesAsync();
        }
    }
}
