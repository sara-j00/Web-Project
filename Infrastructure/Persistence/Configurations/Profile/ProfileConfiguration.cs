using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Profile;

public class ProfileConfiguration : IEntityTypeConfiguration<Domain.Entities.Profile>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Profile> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.HasOne<IdentityUser>()
            .WithOne()
            .HasForeignKey<Domain.Entities.Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Username)
            .HasMaxLength(100);

        builder.Property(p => p.MobileNumber)
            .HasMaxLength(10);
    }
}
