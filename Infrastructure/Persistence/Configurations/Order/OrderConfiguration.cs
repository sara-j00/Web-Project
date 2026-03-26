using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations.Order;

public class OrderConfiguration : IEntityTypeConfiguration<Domain.Entities.Order>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Total)
            .HasColumnType("decimal(18,2)");

        builder.HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
