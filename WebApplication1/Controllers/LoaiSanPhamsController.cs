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
    public class LoaiSanPhamsController : ControllerBase
    {
        private readonly SPContext _context;

        public LoaiSanPhamsController(SPContext context)
        {
            _context = context;
        }

        // GET: api/LoaiSanPhams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiSanPham>>> GetLoaiSanPhams(string? keyword, int start = 0, int length = 10)
        {
            var query = _context.LoaiSanPhams
            .Select(lsp => new
            {
                lsp.LoaiSanPhamId,
                lsp.TenLoai,
                SoLuongSanPham = lsp.SanPhams.Count,
                lsp.NgayNhap
            });

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.TenLoai.Contains(keyword));
            }



            var totalRecords = query.Count();
            var data = query
                .Skip(start)
                .Take(length)
                .ToList();

            return Ok(new
            {
                draw = HttpContext.Request.Query["draw"],
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data
            });
        }

            // GET: api/LoaiSanPhams/5
            [HttpGet("{id}")]
        public async Task<ActionResult<LoaiSanPham>> GetLoaiSanPham(int id)
        {
          if (_context.LoaiSanPhams == null)
          {
              return NotFound();
          }
            var loaiSanPham = await _context.LoaiSanPhams.FindAsync(id);

            if (loaiSanPham == null)
            {
                return NotFound();
            }

            return loaiSanPham;
        }

        // PUT: api/LoaiSanPhams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoaiSanPham(int id, [FromBody] AddLoaiSanPham updatedLoaiSanPham)
        {
            updatedLoaiSanPham.NgayNhap = DateTime.Parse("1/5/2025");
            if (id != updatedLoaiSanPham.LoaiSanPhamId)
            {
                return BadRequest(new { success = false, message = "ID loại sản phẩm không khớp." });
            }

            if (string.IsNullOrEmpty(updatedLoaiSanPham.TenLoai))
            {
                return BadRequest(new { success = false, message = "Tên loại sản phẩm không được để trống." }); 

            }

            if (updatedLoaiSanPham.NgayNhap == null || updatedLoaiSanPham.NgayNhap > DateTime.Now)
            {
                return BadRequest(new { success = false, message = "Ngày nhập không hợp lệ." });
            }

            var existingSanPham = await _context.LoaiSanPhams.FirstOrDefaultAsync(x => x.LoaiSanPhamId == id);

            if (existingSanPham == null)
            {
                return NotFound(new { success = false, message = "Loại sản phẩm không tồn tại." });
            }

            existingSanPham.TenLoai = updatedLoaiSanPham.TenLoai;
            existingSanPham.NgayNhap = updatedLoaiSanPham.NgayNhap;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật loại sản phẩm thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật loại sản phẩm.", error = ex.Message });
            }
        }

        // POST: api/LoaiSanPhams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoaiSanPham>> PostLoaiSanPham([FromBody] AddLoaiSanPham loaiSanPham)
        {
            loaiSanPham.NgayNhap = DateTime.Parse("1/5/2025");
            if (loaiSanPham == null)
            {
                return BadRequest(new { success = false, message = "Tên loại sản phẩm không được để trống." });
            }

            if (string.IsNullOrEmpty(loaiSanPham.TenLoai))
            {
                return BadRequest(new { success = false, message = "Tên loại sản phẩm không được để trống." });

            }
            else
            {

                var existingSanPham = await _context.LoaiSanPhams
                    .FirstOrDefaultAsync(x => x.TenLoai == loaiSanPham.TenLoai);
                if (existingSanPham != null)
                {
                    return BadRequest(new { success = false, message = "Tên loại sản phẩm đã tồn tại." });
                    //ModelState.AddModelError("TenSanPham", "Tên sản phẩm đã tồn tại.");
                }
            }


            if (loaiSanPham.NgayNhap == null || loaiSanPham.NgayNhap > DateTime.Now)
            {
                return BadRequest(new { success = false, message = "Ngày nhập không hợp lệ." });

            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var lSanPham = new LoaiSanPham
            {
                TenLoai = loaiSanPham.TenLoai,
                NgayNhap = loaiSanPham.NgayNhap
            };

            _context.LoaiSanPhams.Add(lSanPham);
            await _context.SaveChangesAsync();


            return Ok(new { success = true, message = "Sản phẩm đã được thêm thành công." });
        }

        // DELETE: api/LoaiSanPhams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoaiSanPham(int id)
        {
            var loaiSanPham = await _context.LoaiSanPhams.Include(l => l.SanPhams).FirstOrDefaultAsync(l => l.LoaiSanPhamId == id);
            if (loaiSanPham == null) return NotFound();

            if (loaiSanPham.SanPhams.Any())
            {
                return BadRequest("Không thể xóa vì loại sản phẩm này có sản phẩm liên kết.");
            }

            _context.LoaiSanPhams.Remove(loaiSanPham);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Xóa thành công." });
        }

        private bool LoaiSanPhamExists(int id)
        {
            return (_context.LoaiSanPhams?.Any(e => e.LoaiSanPhamId == id)).GetValueOrDefault();
        }
    }
}
