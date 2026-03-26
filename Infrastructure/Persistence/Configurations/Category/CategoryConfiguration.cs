using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations.Category;

public class CategoryConfiguration : IEntityTypeConfiguration<Domain.Entities.Category>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.Category> builder)
    {

        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
