using EduPlatform.Application.Features.Courses.Commands;
using FluentValidation;

namespace EduPlatform.Application.Features.Courses.Validators;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.SubjectId).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Level).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Language).NotEmpty().MaximumLength(50);
    }
}
