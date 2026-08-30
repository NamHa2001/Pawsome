using Microsoft.EntityFrameworkCore;
using Pawsome.Infrastructure;

namespace Pawsome.API.Services.GioHang
{
    public class PawVipService : IPawVipService
    {
        private readonly PawsomeDbContext _context;

        public PawVipService(PawsomeDbContext context)
        {
            _context = context;
        }

        public async Task<(string? Tier, DateOnly? HetHan)> LayTrangThaiAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new { u.PawVipTier, u.PawVipHetHan })
                .FirstOrDefaultAsync();

            if (user == null) return (null, null);

            var homNay = DateOnly.FromDateTime(DateTime.UtcNow);
            if (!PawVipTiers.ConHieuLuc(user.PawVipTier, user.PawVipHetHan, homNay))
                return (null, null);

            return (user.PawVipTier, user.PawVipHetHan);
        }

        public async Task<string> KichHoatAsync(int userId, string tier)
        {
            if (!PawVipTiers.PhanTramGiam.ContainsKey(tier))
                throw new InvalidOperationException("Gói PawVip không hợp lệ.");

            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            // Còn hạn (kể cả đang đổi sang tier khác) thì cộng dồn thêm 1 năm từ hạn cũ, không
            // ghè đè mất thời gian còn lại. Hết hạn hoặc chưa từng mua thì tính từ hôm nay.
            var homNay = DateOnly.FromDateTime(DateTime.UtcNow);
            var conHieuLuc = PawVipTiers.ConHieuLuc(user.PawVipTier, user.PawVipHetHan, homNay);
            var mocBatDau = conHieuLuc ? user.PawVipHetHan!.Value : homNay;

            user.PawVipTier = tier;
            user.PawVipHetHan = mocBatDau.AddYears(1);
            await _context.SaveChangesAsync();

            return tier;
        }
    }
}
