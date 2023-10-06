using AngleSharp.Dom;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Blaztore.Examples.Wasm.Tests;

public class ElementAssertions : ReferenceTypeAssertions<IElement, ElementAssertions>
{
    private readonly IElement _subject;

    public ElementAssertions(IElement subject) : base(subject) => _subject = subject;

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