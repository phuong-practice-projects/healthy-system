using System.ComponentModel.DataAnnotations;

namespace Healthy.Domain.Common;

/// <summary>
/// Base entity class with basic properties
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the EntityBase class
    /// </summary>
    protected EntityBase()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of the EntityBase class with specified id
    /// </summary>
    /// <param name="id">The unique identifier</param>
    protected EntityBase(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Determines whether the current entity is equal to another entity
    /// </summary>
    /// <param name="obj">The object to compare with the current entity</param>
    /// <returns>true if the entities are equal; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Returns the hash code for this entity
    /// </summary>
    /// <returns>A hash code for the current entity</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Determines whether two entities are equal
    /// </summary>
    /// <param name="left">The first entity to compare</param>
    /// <param name="right">The second entity to compare</param>
    /// <returns>true if the entities are equal; otherwise, false</returns>
    public static bool operator ==(EntityBase? left, EntityBase? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two entities are not equal
    /// </summary>
    /// <param name="left">The first entity to compare</param>
    /// <param name="right">The second entity to compare</param>
    /// <returns>true if the entities are not equal; otherwise, false</returns>
    public static bool operator !=(EntityBase? left, EntityBase? right)
    {
        return !Equals(left, right);
    }
}
