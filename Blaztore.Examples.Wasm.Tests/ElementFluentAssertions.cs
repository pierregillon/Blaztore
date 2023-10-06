using AngleSharp.Dom;

namespace Blaztore.Examples.Wasm.Tests;

public static class ElementFluentAssertions
{
    public static ElementAssertions Should(this IElement element) => new(element);
}