using AirportTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AirportTool.Infrastructure.Persistence.Configurations;

public class GateConfiguration : IEntityTypeConfiguration<Gate>
{
    public void Configure(EntityTypeBuilder<Gate> builder)
    {
        builder.HasKey(g => g.GateId);

        builder.Property(g => g.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(g => new { g.AirportId, g.Code })
            .IsUnique();

        builder.HasOne(g => g.Airport)
            .WithMany(a => a.Gates)
            .HasForeignKey(g => g.AirportId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
