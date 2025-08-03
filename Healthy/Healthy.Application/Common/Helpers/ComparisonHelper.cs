namespace Healthy.Application.Common.Helpers;

public static class ComparisonHelper
{
    /// <summary>
    /// Case-insensitive GUID comparison for Entity Framework queries with string UserId
    /// </summary>
    public static IQueryable<T> WhereUserIdEquals<T>(this IQueryable<T> query, string userId, Func<T, Guid> userIdSelector)
    {
        var userIdString = userId.ToLower();
        return query.Where(entity => userIdSelector(entity).ToString().ToLower() == userIdString);
    }

    /// <summary>
    /// Case-insensitive GUID comparison for Entity Framework queries with Guid UserId
    /// </summary>
    public static IQueryable<T> WhereUserIdEquals<T>(this IQueryable<T> query, Guid userId, Func<T, Guid> userIdSelector)
    {
        var userIdString = userId.ToString().ToLower();
        return query.Where(entity => userIdSelector(entity).ToString().ToLower() == userIdString);
    }

    /// <summary>
    /// Case-insensitive GUID comparison using string
    /// </summary>
    public static bool EqualsIgnoreCase(this Guid guid1, Guid guid2)
    {
        return guid1.ToString().ToLower() == guid2.ToString().ToLower();
    }

    /// <summary>
    /// Case-insensitive GUID comparison with string
    /// </summary>
    public static bool EqualsIgnoreCase(this Guid guid, string guidString)
    {
        return guid.ToString().Equals(guidString, StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// Normalize GUID to lowercase string
    /// </summary>
    public static string ToLowerString(this Guid guid)
    {
        return guid.ToString().ToLower();
    }

    /// <summary>
    /// Case-insensitive string comparison with GUID
    /// </summary>
    public static bool EqualsIgnoreCase(this string guidString, Guid guid)
    {
        return guidString.ToLower() == guid.ToString().ToLower();
    }

    /// <summary>
    /// Case-insensitive string comparison
    /// </summary>
    public static bool EqualsIgnoreCase(this string str1, string str2)
    {
        return str1?.ToLower() == str2?.ToLower();
    }

    /// <summary>
    /// Check if string is a valid GUID format (ignore case)
    /// </summary>
    public static bool IsValidGuid(this string guidString)
    {
        return Guid.TryParse(guidString, out _);
    }

    /// <summary>
    /// Try parse string to GUID (ignore case)
    /// </summary>
    public static Guid? ToGuidOrNull(this string guidString)
    {
        return Guid.TryParse(guidString, out var result) ? result : null;
    }

    /// <summary>
    /// Try parse string to GUID with default value
    /// </summary>
    public static Guid ToGuidOrDefault(this string guidString, Guid defaultValue = default)
    {
        return Guid.TryParse(guidString, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Compare GUID with nullable GUID (ignore case)
    /// </summary>
    public static bool EqualsIgnoreCase(this Guid guid, Guid? nullableGuid)
    {
        return nullableGuid.HasValue && guid.EqualsIgnoreCase(nullableGuid.Value);
    }

    /// <summary>
    /// Compare nullable GUID with GUID (ignore case)
    /// </summary>
    public static bool EqualsIgnoreCase(this Guid? nullableGuid, Guid guid)
    {
        return nullableGuid.HasValue && nullableGuid.Value.EqualsIgnoreCase(guid);
    }

    /// <summary>
    /// Compare two nullable GUIDs (ignore case)
    /// </summary>
    public static bool EqualsIgnoreCase(this Guid? guid1, Guid? guid2)
    {
        if (!guid1.HasValue && !guid2.HasValue) return true;
        if (!guid1.HasValue || !guid2.HasValue) return false;
        return guid1.Value.EqualsIgnoreCase(guid2.Value);
    }
}
