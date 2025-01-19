using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class SanPham
    {
        public SanPham()
        {
            LoaiSanPhams = new HashSet<LoaiSanPham>();
        }

        public int SanPhamId { get; set; }
        public string? TenSanPham { get; set; }
        public decimal? Gia { get; set; }
        public DateTime? NgayNhap { get; set; }

        public virtual ICollection<LoaiSanPham> LoaiSanPhams { get; set; }
    }

    public partial class AddSanPham
    {
        

        public int SanPhamId { get; set; }
        public string? TenSanPham { get; set; }
        public decimal? Gia { get; set; }
        public DateTime? NgayNhap { get; set; }

        //public virtual ICollection<LoaiSanPham> LoaiSanPhams { get; set; }
    }
}
