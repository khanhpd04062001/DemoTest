using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamsController : ControllerBase
    {
        private readonly SPContext _context;

        public SanPhamsController(SPContext context)
        {
            _context = context;
        }

        // GET: api/SanPhams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetSanPhams(string? keyword, int? loaiSanPhamId, int start = 0, int length = 100)
        {
            var query = _context.SanPhams
            .Include(sp => sp.LoaiSanPhams)
            .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(sp => sp.TenSanPham.Contains(keyword));
            }

            if (loaiSanPhamId.HasValue)
            {
                query = query.Where(sp => sp.LoaiSanPhams.Any(x => x.LoaiSanPhamId == loaiSanPhamId));
            }

            var totalRecords = query.Count();

            var data = query
                .Skip(start)
                .Take(length)
                .Select(sp => new
                {
                    sp.SanPhamId,
                    sp.TenSanPham,
                    sp.Gia,
                    TenLoai = string.Join(", ", sp.LoaiSanPhams.Select(l => l.TenLoai)),
                    sp.NgayNhap
                })
                .ToList();

            return Ok(new
            {
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data
            });
            
        }

        // GET: api/SanPhams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPham>> GetSanPham(int id)
        {
          if (_context.SanPhams == null)
          {
              return NotFound();
          }
            var sanPham = await _context.SanPhams.FindAsync(id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return sanPham;
        }

        // PUT: api/SanPhams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSanPham(int id, [FromBody] AddSanPham updatedSanPham)
        {
            if (id != updatedSanPham.SanPhamId)
            {
                return BadRequest(new { success = false, message = "ID sản phẩm không khớp." });
            }

            
            if (string.IsNullOrEmpty(updatedSanPham.TenSanPham))
            {
                return BadRequest(new { success = false, message = "Tên sản phẩm không được để trống." });
            }

            if (updatedSanPham.NgayNhap == null || updatedSanPham.NgayNhap > DateTime.Now)
            {
                return BadRequest(new { success = false, message = "Ngày nhập không hợp lệ." });
            }

            var existingSanPham = await _context.SanPhams.FirstOrDefaultAsync(x => x.SanPhamId == id);

            if (existingSanPham == null)
            {
                return NotFound(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            // Cập nhật dữ liệu
            existingSanPham.TenSanPham = updatedSanPham.TenSanPham;
            existingSanPham.Gia = updatedSanPham.Gia;
            existingSanPham.NgayNhap = updatedSanPham.NgayNhap;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật sản phẩm thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật sản phẩm.", error = ex.Message });
            }
        }

        // POST: api/SanPhams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SanPham>> PostSanPham([FromBody]AddSanPham sp)
        {
            sp.NgayNhap = DateTime.Parse("1/5/2025");
            if (sp == null)
            {
                return BadRequest(new { success = false, message = "Tên sản phẩm không được để trống." });
            }

            if (string.IsNullOrEmpty(sp.TenSanPham))
            {
                return BadRequest(new { success = false, message = "Tên sản phẩm không được để trống." });
                
            }
            else
            {
                
                var existingSanPham = await _context.SanPhams
                    .FirstOrDefaultAsync(x => x.TenSanPham == sp.TenSanPham);
                if (existingSanPham != null)
                {
                    return BadRequest(new { success = false, message = "Tên sản phẩm đã tồn tại." });
                    //ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại.");
                }
            }

            
            if (sp.NgayNhap == null || sp.NgayNhap > DateTime.Now)
            {
                return BadRequest(new { success = false, message = "Ngày nhập không hợp lệ." });
               
            }

            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var sanPham = new SanPham
            {
                TenSanPham = sp.TenSanPham,
                Gia = sp.Gia,
                NgayNhap = sp.NgayNhap
            };

            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();


            return Ok(new { success = true, message = "Sản phẩm đã được thêm thành công." });
        }

        // DELETE: api/SanPhams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSanPham(int id)
        {
            if (_context.SanPhams == null)
            {
                return NotFound();
            }
            var sanPham = await _context.SanPhams.FirstOrDefaultAsync(x => x.SanPhamId == id);
            //_context.Entry(sanPham).State = EntityState.Deleted;

            if (sanPham == null)
            {
                return NotFound();
            }
            sanPham.LoaiSanPhams.Clear();
            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa sản phẩm thành công." });
        }

        private bool SanPhamExists(int id)
        {
            return (_context.SanPhams?.Any(e => e.SanPhamId == id)).GetValueOrDefault();
        }
    }
}
