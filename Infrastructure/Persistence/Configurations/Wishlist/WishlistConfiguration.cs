using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Domain;

namespace Infrastructure.Persistence.Configurations.Wishlist;

public class WishlistConfiguration : IEntityTypeConfiguration<Domain.Entities.Wishlist>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Wishlist> builder)
    {
        builder.HasKey(w => w.Id);

        builder.HasIndex(w => w.ProfileId).IsUnique();

        builder.HasOne(w => w.Profile)
               .WithOne(p => p.Wishlist)
               .HasForeignKey<Domain.Entities.Wishlist>(w => w.ProfileId)
               .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(w => w.Items)
            .WithOne(i => i.Wishlist)
            .HasForeignKey(i => i.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
