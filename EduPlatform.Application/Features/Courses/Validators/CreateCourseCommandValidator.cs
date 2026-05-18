using EduPlatform.Application.Features.Courses.Commands;
using FluentValidation;

namespace EduPlatform.Application.Features.Courses.Validators;

/// <summary>
/// FluentValidation rules for creating a new course.
///
/// Why FluentValidation instead of DataAnnotations?
/// DataAnnotations are attribute-based and cannot express:
/// - Conditional rules: VideoUrl is required only for Video lessons (.When())
/// - Cross-property rules: ConfirmPassword must equal Password
/// - Async rules: check database for duplicate title
/// - Readable rule chaining: multiple rules per property, each with its own message
///
/// Registration:
/// This validator is auto-discovered by ApplicationServiceRegistration.cs:
///   services.AddValidatorsFromAssembly(typeof(MappingProfile).Assembly);
/// No manual registration is needed.
///
/// Where it runs:
/// 1. API layer: FluentValidation.AspNetCore wires it into ModelState automatically.
///    ValidateModelFilter converts ModelState errors to ApiResponse.Fail().
/// 2. Application layer: ValidationBehavior&lt;CreateCourseCommand&gt; can be called
///    inside CourseService to validate before executing business logic.
///
/// Testing:
/// Use FluentValidation.TestHelper:
///   var result = _validator.TestValidate(new CreateCourseCommand { Price = -1 });
///   result.ShouldHaveValidationErrorFor(x => x.Price);
/// </summary>
public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    private static readonly string[] ValidLevels    = ["Beginner", "Intermediate", "Advanced"];
    private static readonly string[] ValidLanguages = ["English", "Arabic", "French"];

    public CreateCourseCommandValidator()
    {
        // ── Title ─────────────────────────────────────────────────────────────
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Course title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        // ── Description ───────────────────────────────────────────────────────
        // Optional field — only validate length if the teacher provided a value.
        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2,000 characters.")
            .When(x => x.Description != null);

        // ── Price ─────────────────────────────────────────────────────────────
        // 0 is valid (free course). No upper limit beyond 10,000 KD.
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.")
            .LessThanOrEqualTo(10_000).WithMessage("Price cannot exceed 10,000 KD.");

        // ── ThumbnailUrl ──────────────────────────────────────────────────────
        // Optional — only validate if provided. Must be an absolute HTTP/HTTPS URL.
        RuleFor(x => x.ThumbnailUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Thumbnail must be a valid absolute URL (https://...).")
            .MaximumLength(500).WithMessage("Thumbnail URL cannot exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.ThumbnailUrl));

        // ── Level ─────────────────────────────────────────────────────────────
        // Enum-style string guard. Must() is used instead of an enum because
        // the display strings may be localized in future.
        RuleFor(x => x.Level)
            .NotEmpty().WithMessage("Level is required.")
            .Must(l => ValidLevels.Contains(l))
            .WithMessage($"Level must be one of: {string.Join(", ", ValidLevels)}.");

        // ── Language ──────────────────────────────────────────────────────────
        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.")
            .Must(l => ValidLanguages.Contains(l))
            .WithMessage($"Language must be one of: {string.Join(", ", ValidLanguages)}.");

        // ── Requirements / LearningOutcomes ───────────────────────────────────
        RuleFor(x => x.Requirements)
            .MaximumLength(2000).WithMessage("Requirements cannot exceed 2,000 characters.")
            .When(x => x.Requirements != null);

        RuleFor(x => x.LearningOutcomes)
            .MaximumLength(2000).WithMessage("Learning outcomes cannot exceed 2,000 characters.")
            .When(x => x.LearningOutcomes != null);

        // ── SubjectId ─────────────────────────────────────────────────────────
        // Must be positive — 0 or negative means the form was submitted without
        // selecting a subject. Business validation (subject exists in DB) lives
        // in CourseService, not here.
        RuleFor(x => x.SubjectId)
            .GreaterThan(0).WithMessage("A valid subject must be selected.");

        // ── CategoryId ────────────────────────────────────────────────────────
        // Optional. If provided, must be a positive integer.
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category ID must be positive.")
            .When(x => x.CategoryId.HasValue);
    }
}
