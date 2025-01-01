using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EndTimeAfterStartTimeAttribute : ValidationAttribute
{
    private readonly string _startTimePropertyName;

    public EndTimeAfterStartTimeAttribute(string startTimePropertyName)
    {
        _startTimePropertyName = startTimePropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var startTimeProperty = validationContext.ObjectType.GetProperty(_startTimePropertyName);
        if (startTimeProperty == null)
        {
            return new ValidationResult($"Property {_startTimePropertyName} not found.");
        }

        var startTimeValue = startTimeProperty.GetValue(validationContext.ObjectInstance) as DateTime?;
        var endTimeValue = value as DateTime?;

        if (!startTimeValue.HasValue || !endTimeValue.HasValue)
        {
            return new ValidationResult("Both StartTime and EndTime are required.");
        }

        if (endTimeValue <= startTimeValue)
        {
            System.Console.WriteLine(startTimeValue);
            System.Console.WriteLine(endTimeValue);
            
            return new ValidationResult("EndTime must be later than StartTime.");
        }

        return ValidationResult.Success;
    }
}
