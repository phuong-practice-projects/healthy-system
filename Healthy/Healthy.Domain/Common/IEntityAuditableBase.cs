namespace Healthy.Domain.Common;

/// <summary>
/// Interface for entities that support audit information
/// </summary>
public interface IEntityAuditableBase
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity
    /// </summary>
    string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was soft deleted
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who soft deleted the entity
    /// </summary>
    string? DeletedBy { get; set; }
}
