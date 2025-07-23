namespace PipeForge.Extensions;

internal static class FilterExtensions
{
    private static readonly StringComparison _comp = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Returns true if the descriptor's filters match any of the provided filters.
    /// If no filters are provided, it returns true by default.
    /// </summary>
    /// <param name="descriptorFilters"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    public static bool MatchesAnyFilter(this IEnumerable<string> descriptorFilters, string[]? filters)
    {
        if (!descriptorFilters.Any()) return true;
        if (filters is null || !filters.Any()) return false;
        return descriptorFilters.Any(df => filters.Any(f => string.Equals(df, f, _comp)));
    }
}
