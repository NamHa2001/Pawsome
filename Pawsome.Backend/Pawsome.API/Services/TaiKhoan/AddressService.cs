using Microsoft.EntityFrameworkCore;
using Pawsome.API.DTOs.TaiKhoan;
using Pawsome.Domain.Entities.TaiKhoan;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.TaiKhoan;

public class AddressService : IAddressService
{
    private readonly PawsomeDbContext _dbContext;

    public AddressService(PawsomeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AddressDto>> GetAllAsync(int userId)
    {
        return await _dbContext.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.LaMacDinh) // địa chỉ mặc định hiện lên đầu danh sách
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task<AddressDto> CreateAsync(int userId, CreateAddressRequestDto dto)
    {
        // Nếu đây là địa chỉ đầu tiên của user, tự động là mặc định dù client không tick
        var isFirstAddress = !await _dbContext.Addresses.AnyAsync(a => a.UserId == userId);
        var laMacDinh = dto.LaMacDinh || isFirstAddress;

        if (laMacDinh)
            await ClearOldDefaultAsync(userId);

        var address = new Address
        {
            UserId = userId,
            NguoiNhan = dto.NguoiNhan,
            SoDienThoai = dto.SoDienThoai,
            DiaChiChiTiet = dto.DiaChiChiTiet,
            PhuongXa = dto.PhuongXa,
            QuanHuyen = dto.QuanHuyen,
            TinhThanh = dto.TinhThanh,
            LaMacDinh = laMacDinh
        };

        _dbContext.Addresses.Add(address);
        await _dbContext.SaveChangesAsync();

        return MapToDto(address);
    }

    public async Task<AddressDto> UpdateAsync(int userId, int addressId, UpdateAddressRequestDto dto)
    {
        var address = await GetOwnedAddressAsync(userId, addressId);

        if (dto.LaMacDinh && !address.LaMacDinh)
            await ClearOldDefaultAsync(userId);

        address.NguoiNhan = dto.NguoiNhan;
        address.SoDienThoai = dto.SoDienThoai;
        address.DiaChiChiTiet = dto.DiaChiChiTiet;
        address.PhuongXa = dto.PhuongXa;
        address.QuanHuyen = dto.QuanHuyen;
        address.TinhThanh = dto.TinhThanh;
        address.LaMacDinh = dto.LaMacDinh;

        await _dbContext.SaveChangesAsync();

        return MapToDto(address);
    }

    public async Task DeleteAsync(int userId, int addressId)
    {
        var address = await GetOwnedAddressAsync(userId, addressId);

        var dangDuocDonHangThamChieu = await _dbContext.Orders.AnyAsync(o => o.AddressId == addressId);
        if (dangDuocDonHangThamChieu)
            throw new InvalidOperationException("Không thể xóa địa chỉ đã được dùng trong đơn hàng.");

        _dbContext.Addresses.Remove(address);
        await _dbContext.SaveChangesAsync();

        // Nếu vừa xóa đúng địa chỉ mặc định, tự động gán mặc định cho địa chỉ còn lại đầu tiên (nếu có)
        if (address.LaMacDinh)
        {
            var conLai = await _dbContext.Addresses
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();

            if (conLai != null)
            {
                conLai.LaMacDinh = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    // Đảm bảo địa chỉ thuộc đúng user đang đăng nhập - chặn user A sửa/xóa địa chỉ của user B
    private async Task<Address> GetOwnedAddressAsync(int userId, int addressId)
    {
        var address = await _dbContext.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

        if (address == null)
            throw new InvalidOperationException("Không tìm thấy địa chỉ.");

        return address;
    }

    private async Task ClearOldDefaultAsync(int userId)
    {
        var oldDefaults = await _dbContext.Addresses
            .Where(a => a.UserId == userId && a.LaMacDinh)
            .ToListAsync();

        foreach (var a in oldDefaults)
            a.LaMacDinh = false;
    }

    private static AddressDto MapToDto(Address a) => new()
    {
        AddressId = a.AddressId,
        NguoiNhan = a.NguoiNhan,
        SoDienThoai = a.SoDienThoai,
        DiaChiChiTiet = a.DiaChiChiTiet,
        PhuongXa = a.PhuongXa,
        QuanHuyen = a.QuanHuyen,
        TinhThanh = a.TinhThanh,
        LaMacDinh = a.LaMacDinh
    };
}