namespace AXDD.Services.Search.Api.Settings;

/// <summary>
/// Configuration settings for search behavior
/// </summary>
public class SearchSettings
{
    /// <summary>
    /// Default page size for search results
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// Maximum allowed page size
    /// </summary>
    public int MaxPageSize { get; set; } = 100;

    /// <summary>
    /// Highlight fragment size in characters
    /// </summary>
    public int HighlightFragmentSize { get; set; } = 150;

    /// <summary>
    /// Enable fuzzy search for typo tolerance
    /// </summary>
    public bool FuzzyEnabled { get; set; } = true;

    /// <summary>
    /// Maximum edit distance for fuzzy search
    /// </summary>
    public int FuzzyMaxEditDistance { get; set; } = 2;

    /// <summary>
    /// Number of suggestions to return for autocomplete
    /// </summary>
    public int SuggestionCount { get; set; } = 10;

    /// <summary>
    /// Minimum term length for fuzzy search
    /// </summary>
    public int MinTermLengthForFuzzy { get; set; } = 3;
}
