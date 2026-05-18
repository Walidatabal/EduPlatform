using EduPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduPlatform.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core mapping for parent-student relationship.
/// </summary>
public class ParentStudentLinkConfiguration : IEntityTypeConfiguration<ParentStudentLink>
{
    public void Configure(EntityTypeBuilder<ParentStudentLink> builder)
    {
        // Soft delete filter follows the same enterprise pattern used by the rest of the entities.
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.ParentId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.StudentId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.RelationshipType)
            .HasMaxLength(50)
            .IsRequired();

        // Prevent duplicated parent/student rows.
        builder.HasIndex(x => new { x.ParentId, x.StudentId })
            .IsUnique();
    }
}
