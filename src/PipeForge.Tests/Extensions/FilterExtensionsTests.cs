using PipeForge.Extensions;

namespace PipeForge.Tests.Extensions;

public class FilterExtensionsTests
{
    [Fact]
    public void MatchesAnyFilter_ReturnsTrue_WhenDescriptorFiltersIsEmpty()
    {
        var descriptorFilters = new string[0];
        var filters = new[] { "foo", "bar" };

        var result = descriptorFilters.MatchesAnyFilter(filters);

        result.ShouldBeTrue();
    }

    [Fact]
    public void MatchesAnyFilter_ReturnsFalse_WhenFiltersIsNullAndDescriptorFiltersIsNotEmpty()
    {
        var descriptorFilters = new[] { "foo", "bar" };
        string[]? filters = null;

        var result = descriptorFilters.MatchesAnyFilter(filters);

        result.ShouldBeFalse();
    }

    [Fact]
    public void MatchesAnyFilter_ReturnsFalse_WhenFiltersIsEmptyAndDescriptorFiltersIsNotEmpty()
    {
        var descriptorFilters = new[] { "foo", "bar" };
        var filters = new string[0];

        var result = descriptorFilters.MatchesAnyFilter(filters);

        result.ShouldBeFalse();
    }

    [Fact]
    public void MatchesAnyFilter_ReturnsTrue_WhenAtLeastOneFilterMatches()
    {
        var descriptorFilters = new[] { "foo", "bar" };
        var filters = new[] { "bar", "baz" };

        var result = descriptorFilters.MatchesAnyFilter(filters);

        result.ShouldBeTrue();
    }

    [Fact]
    public void MatchesAnyFilter_ReturnsTrue_WhenMatchIsCaseInsensitive()
    {
        var descriptorFilters = new[] { "FoO", "BaR" };
        var filters = new[] { "foo" };

        var result = descriptorFilters.MatchesAnyFilter(filters);

        result.ShouldBeTrue();
    }

    [Fact]
    public void MatchesAnyFilter_ReturnsFalse_WhenNoFiltersMatch()
    {
        var descriptorFilters = new[] { "foo", "bar" };
        var filters = new[] { "baz", "qux" };

        var result = descriptorFilters.MatchesAnyFilter(filters);

        result.ShouldBeFalse();
    }
}
