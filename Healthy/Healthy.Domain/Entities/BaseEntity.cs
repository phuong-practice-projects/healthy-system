using Healthy.Domain.Common;

namespace Healthy.Domain.Entities;

/// <summary>
/// Legacy base entity class - inherits from EntityAuditableBase
/// Use EntityAuditableBase directly for new entities
/// </summary>
[Obsolete("Use EntityAuditableBase from Healthy.Domain.Common instead")]
public abstract class BaseEntity : EntityAuditableBase
{
    /// <summary>
    /// Initializes a new instance of the BaseEntity class
    /// </summary>
    protected BaseEntity() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseEntity class with specified id
    /// </summary>
    /// <param name="id">The unique identifier</param>
    protected BaseEntity(Guid id) : base(id)
    {
    }
} 