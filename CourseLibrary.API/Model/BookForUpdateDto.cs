using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Model
{
    public class BookForUpdateDto : BookForManipulationDto
    {
        //to reduce duplication...we can create an abstract class that will server as the Base Class for both of these dtos
        // (we make the class abstract so that it can't function on its own..must be derived from)

        /* [Required]
         [MaxLength(100, ErrorMessage = "You should fill out a title. ")]
         public string Title { get; set; }

         [Required(ErrorMessage ="The Description is required")]
         [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
         public string Description { get; set; } */
        public override string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
