using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduPlatform.Infrastructure.Configurations;

public class CourseQuestionConfiguration : IEntityTypeConfiguration<CourseQuestion>
{
    public void Configure(EntityTypeBuilder<CourseQuestion> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.HasOne(x => x.Course)
            .WithMany()
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
