using EduPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduPlatform.Infrastructure.Persistence.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Section)
            .WithMany(x => x.Lessons)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
