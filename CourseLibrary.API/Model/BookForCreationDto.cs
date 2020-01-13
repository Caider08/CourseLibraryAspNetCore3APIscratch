using CourseLibrary.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Model
{
    [BookTitleMustBeDifferentFromDescriptionAttribute(ErrorMessage = "Title must be different from description.")]
    public class BookForCreationDto : BookForManipulationDto //:  IValidatableObject
    {
        /* [Required]
         [MaxLength(100, ErrorMessage = "You should fill out a title. ")]
         public string Title { get; set; }

         [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
         public string Description { get; set; }
         */

        //Our class-level attribute at the top takes care of the below:
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Title == Description)
        //    {
        //        yield return new ValidationResult(
        //            "The provided description should be different from the title.",
        //            new[] { "BookForCreationDto" });
        //    }
        //}
        //if that base class property was marked as abstract
        //public override string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
