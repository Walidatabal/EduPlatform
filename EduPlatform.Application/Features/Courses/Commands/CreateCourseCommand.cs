namespace EduPlatform.Application.Features.Courses.Commands;

/// <summary>
/// Command object representing the data required to create a new course.
///
/// What is a Command?
/// A Command is an input model for a write operation. It carries only the data
/// that the client (API or MVC form) submits — never internal fields like TeacherId
/// (set from the authenticated user) or Status (always Draft on creation).
///
/// Why no DataAnnotations?
/// DataAnnotations ([Required], [MaxLength] etc.) mix validation rules into the
/// model class, making them hard to unit test and impossible to conditionally apply.
/// All validation for this command lives in CreateCourseCommandValidator.
/// The validator is auto-discovered by AddValidatorsFromAssembly().
///
/// DataAnnotations were intentionally removed from this class.
/// Do not re-add them.
/// </summary>
public class CreateCourseCommand
{
    /// <summary>
    /// Course display title shown in the catalog and course detail page.
    /// Required. Min 3 chars, max 200 chars.
    /// Validated by CreateCourseCommandValidator.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Long-form description for the course detail page.
    /// Optional. Max 2000 chars when provided.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Course price in Kuwaiti Dinar (KD).
    /// 0 = free course. Min 0, max 10,000 KD.
    /// Validated by CreateCourseCommandValidator.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Absolute URL to the course cover image.
    /// Optional. When provided, must be a valid absolute URI (https://...).
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Difficulty level. Must be one of: Beginner, Intermediate, Advanced.
    /// Validated by CreateCourseCommandValidator with a Must() enum guard.
    /// </summary>
    public string Level { get; set; } = "Beginner";

    /// <summary>
    /// Primary language of the course content. Must be one of: English, Arabic, French.
    /// Validated by CreateCourseCommandValidator.
    /// </summary>
    public string Language { get; set; } = "English";

    /// <summary>
    /// Prerequisites the student should meet before enrolling.
    /// Optional. Displayed in the "Requirements" section of the detail page.
    /// </summary>
    public string? Requirements { get; set; }

    /// <summary>
    /// What the student will learn by completing this course.
    /// Optional. Displayed as a bullet list on the course detail page.
    /// </summary>
    public string? LearningOutcomes { get; set; }

    /// <summary>
    /// Optional category ID for secondary classification beyond Grade/Subject.
    /// When provided, must be positive and reference an existing Category.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// The subject this course belongs to.
    /// Required. Must be a positive integer referencing an existing Subject.
    /// The subject determines which Grade the course falls under.
    /// Business validation (subject exists) is handled in CourseService.
    /// </summary>
    public int SubjectId { get; set; }
}
