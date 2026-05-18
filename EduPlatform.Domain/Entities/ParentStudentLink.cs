using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

/// <summary>
/// Links a parent user account to a student user account.
///
/// Purpose in the EduPlatform school LMS:
/// Schools need parents to monitor their children's learning activity.
/// This entity establishes the relationship so the platform can:
/// - Show a parent which courses their child is enrolled in.
/// - Display the child's progress and completion percentage.
/// - Notify the parent when the child earns a certificate.
/// - Allow admins to see the full family relationship structure.
///
/// Architectural decision — why string IDs instead of navigation properties:
/// ASP.NET Core Identity's ApplicationUser lives in the Infrastructure layer.
/// The Domain layer must not reference Infrastructure. Therefore this entity
/// stores the Identity user GUIDs as plain strings rather than navigation
/// properties to ApplicationUser. This keeps Domain pure and independently testable.
///
/// How it is resolved:
/// RelationsController loads the relevant ApplicationUser records separately
/// from UserManager<ApplicationUser> and joins them with the ParentStudentLink
/// records in memory, then maps to ParentStudentRelationVM for display.
///
/// Relationship type:
/// <see cref="RelationshipType"/> is a display string (Father, Mother, Guardian,
/// Grandfather, etc.) rather than an enum because relationship terms vary by
/// culture and context. FluentValidation enforces valid values in the validator.
/// </summary>
public class ParentStudentLink : BaseEntity
{
    /// <summary>
    /// Identity GUID (string) of the parent ApplicationUser.
    /// Validated as a GUID format by CreateParentStudentLinkCommandValidator.
    /// </summary>
    public string ParentId { get; set; } = string.Empty;

    /// <summary>
    /// Identity GUID (string) of the student ApplicationUser.
    /// Must be different from ParentId — validator enforces NotEqual(x => x.ParentId).
    /// </summary>
    public string StudentId { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable relationship description.
    /// Examples: Father, Mother, Guardian, Grandfather, Aunt, Sibling.
    /// Default is Guardian — the most generic and neutral term.
    /// Displayed in the Relations page table and in parent-facing dashboards.
    /// </summary>
    public string RelationshipType { get; set; } = "Guardian";
}
