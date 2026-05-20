using EduPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduPlatform.Infrastructure.Configurations;

public class CourseAnswerConfiguration : IEntityTypeConfiguration<CourseAnswer>
{
    public void Configure(EntityTypeBuilder<CourseAnswer> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.CourseQuestion)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.CourseQuestionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
