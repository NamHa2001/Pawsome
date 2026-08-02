using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pawsome.Domain.Entities.BlogQuanTri;

namespace Pawsome.Infrastructure.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("wishlists");
        builder.HasKey(w => w.WishlistId);

        builder.Property(w => w.WishlistId).HasColumnName("wishlist_id");
        builder.Property(w => w.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(w => w.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(w => w.NgayThem).HasColumnName("ngay_them").HasDefaultValueSql("GETDATE()");

        builder.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique().HasDatabaseName("UQ_wishlists_user_product");
        builder.HasIndex(w => w.UserId).HasDatabaseName("IX_wishlists_user");
        builder.HasIndex(w => w.ProductId).HasDatabaseName("IX_wishlists_product");

        builder.HasOne(w => w.User)
               .WithMany(u => u.Wishlists)
               .HasForeignKey(w => w.UserId)
               .HasConstraintName("FK_wishlists_users")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Product)
               .WithMany(p => p.Wishlists)
               .HasForeignKey(w => w.ProductId)
               .HasConstraintName("FK_wishlists_products")
               .OnDelete(DeleteBehavior.Cascade);
    }
}
