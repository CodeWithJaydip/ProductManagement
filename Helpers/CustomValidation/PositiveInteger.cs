using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Helpers.CustomValidation
{
    public class PositiveInteger: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value is required.");
            }

            // Check if the value is an integer
            if (value is int intValue)
            {
                // Ensure the integer is positive
                if (intValue > 0)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult("The value must be a positive integer.");
            }

            // If the value is not an integer, return an error
            return new ValidationResult("The value must be an integer.");
        }
    }
}
