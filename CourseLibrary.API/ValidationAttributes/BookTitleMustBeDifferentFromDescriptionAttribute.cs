using CourseLibrary.API.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
    public class BookTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var book = (BookForManipulationDto)validationContext.ObjectInstance;

            if (book.Title == book.Description)
            {
                return new ValidationResult(
                    ErrorMessage,
                        new[] { nameof(BookForManipulationDto) });
            }

            return ValidationResult.Success;
        }


    }
}
