﻿using IbanNet.Registry.Patterns;

namespace IbanNet.Registry.Swift
{
    public class SwiftPatternTests
    {
        [Fact]
        public void When_creating_with_null_pattern_it_should_throw()
        {
            string pattern = null;

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            Func<SwiftPattern> act = () => new SwiftPattern(pattern);

            // Assert
            act.Should()
                .ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should()
                .Be(nameof(pattern));
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void Given_pattern_when_calling_to_string_should_return_same_pattern(string pattern, IEnumerable<PatternToken> _)
        {
            // Act
            var actual = new SwiftPattern(pattern);

            // Assert
            actual.ToString().Should().Be(pattern);
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void Given_pattern_tokens_when_calling_to_string_should_return_same_pattern(string expectedPattern, IEnumerable<PatternToken> tokens)
        {
            // Act
            var actual = new SwiftPattern(tokens);

            // Assert
            actual.ToString().Should().Be(expectedPattern);
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void Given_pattern_when_getting_tokens_it_should_return_expected(string pattern, IEnumerable<PatternToken> expectedTokens)
        {
            // Act
            var actual = new SwiftPattern(pattern);

            // Assert
            actual.Tokens.Should().BeEquivalentTo(expectedTokens);
        }

        [Theory]
        [InlineData("2!n4!a4!n2!n8!c", true)]
        [InlineData("2!n4!a4!n2n8!c", false)]
        [InlineData("4!n", true)]
        [InlineData("4n", false)]
        public void Given_pattern_when_getting_isFixedLength_it_should_return_expected(string pattern, bool expectedIsFixedLength)
        {
            // Act
            var actual = new SwiftPattern(pattern);

            // Assert
            actual.IsFixedLength.Should().Be(expectedIsFixedLength);
        }

        public static IEnumerable<object[]> GetTestCases()
        {
            yield return new object[]
            {
                "2!n4!a4!n2!n8!c",
                new List<PatternToken>
                {
                    new(AsciiCategory.Digit, 2),
                    new(AsciiCategory.UppercaseLetter, 4),
                    new(AsciiCategory.Digit, 4),
                    new(AsciiCategory.Digit, 2),
                    new(AsciiCategory.AlphaNumeric, 8)
                }
            };

            yield return new object[]
            {
                "4!n10a1!e2!c",
                new List<PatternToken>
                {
                    new(AsciiCategory.Digit, 4),
                    new(AsciiCategory.UppercaseLetter, 1, 10),
                    new(AsciiCategory.Space, 1),
                    new(AsciiCategory.AlphaNumeric, 2)
                }
            };
        }
    }
}
