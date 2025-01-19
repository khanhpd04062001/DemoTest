using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class LoaiSanPham
    {
        public LoaiSanPham()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int LoaiSanPhamId { get; set; }
        public string? TenLoai { get; set; }
        public DateTime? NgayNhap { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }

    public partial class AddLoaiSanPham
    {
        

        public int LoaiSanPhamId { get; set; }
        public string? TenLoai { get; set; }
        public DateTime? NgayNhap { get; set; }

        
    }
}
