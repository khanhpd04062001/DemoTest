CREATE DATABASE SP
GO
USE SP
GO
CREATE TABLE SanPham (
    SanPhamID INT PRIMARY KEY,
    TenSanPham NVARCHAR(255) ,
    Gia DECIMAL(18, 2) ,
    NgayNhap DATE 
);

CREATE TABLE LoaiSanPham (
    LoaiSanPhamID INT PRIMARY KEY,
    TenLoai NVARCHAR(255) ,
    NgayNhap DATE 
);

CREATE TABLE SanPham_LoaiSanPham (
    SanPhamID INT ,
    LoaiSanPhamID INT ,
    PRIMARY KEY (SanPhamID, LoaiSanPhamID),
    FOREIGN KEY (SanPhamID) REFERENCES SanPham(SanPhamID),
    FOREIGN KEY (LoaiSanPhamID) REFERENCES LoaiSanPham(LoaiSanPhamID)
);