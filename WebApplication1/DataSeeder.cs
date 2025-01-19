using System;
using WebApplication1.Models;

namespace WebApplication1
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(SPContext context)
        {
            // Kiểm tra nếu dữ liệu đã tồn tại
            if (context.LoaiSanPhams.Any() || context.SanPhams.Any())
                return;

            var random = new Random();

            // Tạo 20 loại sản phẩm
            var loaiSanPhams = new List<LoaiSanPham>();
            for (int i = 1; i <= 20; i++)
            {
                loaiSanPhams.Add(new LoaiSanPham
                {
                    TenLoai = $"Loại sản phẩm {i}",
                    NgayNhap = DateTime.Now.AddDays(-random.Next(1, 100))
                });
            }

            await context.LoaiSanPhams.AddRangeAsync(loaiSanPhams);
            await context.SaveChangesAsync();

            // Tạo 10,000 sản phẩm
            var sanPhams = new List<SanPham>();
            for (int i = 1; i <= 10_000; i++)
            {
                var sanPham = new SanPham
                {
                    TenSanPham = $"Sản phẩm {i}",
                    Gia = random.Next(1_000, 100_000),
                    NgayNhap = DateTime.Now.AddDays(-random.Next(1, 100))
                };

                // Gán ngẫu nhiên loại sản phẩm (1-3 loại mỗi sản phẩm)
                var loaiNgauNhien = loaiSanPhams.OrderBy(x => random.Next()).Take(random.Next(1, 4)).ToList();
                sanPham.LoaiSanPhams = loaiNgauNhien;

                sanPhams.Add(sanPham);
            }

            await context.SanPhams.AddRangeAsync(sanPhams);
            await context.SaveChangesAsync();
        }
    }
}
