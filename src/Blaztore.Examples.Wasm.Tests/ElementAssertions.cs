using AngleSharp.Dom;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Blaztore.Examples.Wasm.Tests;

public class ElementAssertions(IElement subject)
    : ReferenceTypeAssertions<IElement, ElementAssertions>(subject, AssertionChain.GetOrCreate())
{
    private readonly IElement _subject = subject;

    public void BeDisabled() => _subject
        .Attributes["disabled"]
        .Should()
        .NotBeNull();

    public void BeEnabled() => _subject
        .Attributes["disabled"]
        .Should()
        .BeNull();

    protected override string Identifier => _subject.Id ?? string.Empty;
}