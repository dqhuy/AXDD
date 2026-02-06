namespace AXDD.BuildingBlocks.Common.Extensions;

/// <summary>
/// String extension methods
/// </summary>
public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string ToSlug(this string value)
    {
        return value.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");
    }
}
