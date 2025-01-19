using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication1.Models
{
    public partial class SPContext : DbContext
    {
        public SPContext()
        {
        }

        public SPContext(DbContextOptions<SPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; } = null!;
        public virtual DbSet<SanPham> SanPhams { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string ConnectionStr = config.GetConnectionString("DB");
                optionsBuilder.UseSqlServer(ConnectionStr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoaiSanPham>(entity =>
            {
                entity.ToTable("LoaiSanPham");

                entity.Property(e => e.LoaiSanPhamId).HasColumnName("LoaiSanPhamID");

                entity.Property(e => e.NgayNhap).HasColumnType("date");

                entity.Property(e => e.TenLoai).HasMaxLength(255);
            });

            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.ToTable("SanPham");

                entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");

                entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayNhap).HasColumnType("date");

                entity.Property(e => e.TenSanPham).HasMaxLength(255);

                entity.HasMany(d => d.LoaiSanPhams)
                    .WithMany(p => p.SanPhams)
                    .UsingEntity<Dictionary<string, object>>(
                        "SanPhamLoaiSanPham",
                        l => l.HasOne<LoaiSanPham>().WithMany().HasForeignKey("LoaiSanPhamId").OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK__SanPham_L__LoaiS__4E88ABD4"),
                        r => r.HasOne<SanPham>().WithMany().HasForeignKey("SanPhamId").OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK__SanPham_L__SanPh__4D94879B"),
                        j =>
                        {
                            j.HasKey("SanPhamId", "LoaiSanPhamId").HasName("PK__SanPham___4FE4723C2393133B");

                            j.ToTable("SanPham_LoaiSanPham");

                            j.IndexerProperty<int>("SanPhamId").HasColumnName("SanPhamID");

                            j.IndexerProperty<int>("LoaiSanPhamId").HasColumnName("LoaiSanPhamID");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
