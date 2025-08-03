namespace Healthy.Domain.Common;

/// <summary>
/// Base entity class with audit information
/// </summary>
public abstract class EntityAuditableBase : EntityBase, IEntityAuditableBase
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was soft deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who soft deleted the entity
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Initializes a new instance of the EntityAuditableBase class
    /// </summary>
    protected EntityAuditableBase() : base()
    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Initializes a new instance of the EntityAuditableBase class with specified id
    /// </summary>
    /// <param name="id">The unique identifier</param>
    protected EntityAuditableBase(Guid id) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks the entity as deleted (soft delete)
    /// </summary>
    /// <param name="deletedBy">The identifier of the user who is deleting the entity</param>
    public virtual void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restores a soft deleted entity
    /// </summary>
    public virtual void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    /// <summary>
    /// Updates the audit information for the entity
    /// </summary>
    /// <param name="updatedBy">The identifier of the user who is updating the entity</param>
    public virtual void UpdateAuditInfo(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
