using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;


namespace Infrastructure.Persistence.Configurations.Cart;

public class CartConfiguration : IEntityTypeConfiguration<Domain.Entities.Cart>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Profile)
            .WithOne(p => p.Cart)
            .HasForeignKey<Domain.Entities.Cart>(c => c.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Items)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.ProfileId)
            .IsRequired();
    }
}
