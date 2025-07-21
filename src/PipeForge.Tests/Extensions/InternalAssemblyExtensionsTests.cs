using System.Reflection;
using PipeForge.Extensions;

namespace PipeForge.Tests.Extensions;

public class InternalAssemblyExtensionsTests
{
    [Fact]
    public void SafeGetTypes_ReturnsTypes_WhenDelegateSucceeds()
    {
        var expected = new[] { typeof(string), typeof(int) };
        var result = InternalAssemblyExtensions.SafeGetTypes(() => expected);
        result.ShouldBe(expected);
    }

    [Fact]
    public void SafeGetTypes_ReturnsFilteredTypes_WhenReflectionTypeLoadExceptionThrown()
    {
        var types = new Type?[] { typeof(string), null, typeof(int) };
        var ex = new ReflectionTypeLoadException(types, []);
        var result = InternalAssemblyExtensions.SafeGetTypes(() => throw ex);
        result.ShouldBe(new[] { typeof(string), typeof(int) });
    }

    [Fact]
    public void SafeGetTypes_ReturnsEmpty_WhenUnexpectedExceptionThrown()
    {
        var result = TestSubject.SafeGetTypes(() => throw new InvalidOperationException());
        result.ShouldBeEmpty();
    }

    private static class TestSubject
    {
        public static IEnumerable<Type> SafeGetTypes(Func<Type[]> getTypes) =>
            InternalAssemblyExtensions.SafeGetTypes(getTypes);
    }
}
