﻿using FluentValidation;
using FluentValidation.Results;
using IbanNet.Validation.Results;
using ValidationResultAlias = FluentValidation.Results.ValidationResult;

namespace IbanNet.FluentValidation
{
    public class IntegrationTests
    {
        private readonly TestModelValidator _sut;
        private readonly TestModel _testModel;

        public IntegrationTests()
        {
            _sut = new TestModelValidator(new IbanValidator());

            _testModel = new TestModel();
        }

        [Theory]
        [InlineData("PL611090101400000712198128741")]
        [InlineData("AE07033123456789012345")]
        public void Given_a_model_with_invalid_iban_when_validating_should_contain_validation_errors(string attemptedIbanValue)
        {
            _testModel.BankAccountNumber = attemptedIbanValue;

            const string expectedFormattedPropertyName = "Bank Account Number";
            var expectedValidationFailure = new ValidationFailure(nameof(_testModel.BankAccountNumber), $"'{expectedFormattedPropertyName}' is not a valid IBAN.")
            {
                AttemptedValue = attemptedIbanValue,
                ErrorCode = "FluentIbanValidator",
                FormattedMessagePlaceholderValues = new Dictionary<string, object>
                {
                    { "PropertyName", expectedFormattedPropertyName },
                    { "PropertyValue", attemptedIbanValue },
                    { "Error", new InvalidLengthResult() }
                }
            };

            // Act
            ValidationResultAlias actual = _sut.Validate(_testModel);

            // Assert
            actual.IsValid.Should().BeFalse("because one validation error should have occurred");
            actual.Errors.Should()
                .HaveCount(1, "because one validation error should have occurred")
                .And.Subject.Single()
                .Should()
                .BeEquivalentTo(expectedValidationFailure);
        }

        [Theory]
        [InlineData("PL61109010140000071219812874")]
        [InlineData("AE070331234567890123456")]
        public void Given_a_model_with_iban_when_validating_should_not_contain_validation_errors(string attemptedIbanValue)
        {
            _testModel.BankAccountNumber = attemptedIbanValue;

            // Act
            ValidationResultAlias actual = _sut.Validate(_testModel);

            // Assert
            actual.IsValid.Should().BeTrue("because no validation errors should have occurred");
        }

        private class TestModelValidator : AbstractValidator<TestModel>
        {
            public TestModelValidator(IIbanValidator ibanValidator)
            {
                RuleFor(x => x.BankAccountNumber).Iban(ibanValidator);
            }
        }
    }
}
