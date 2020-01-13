using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Model
{
    public abstract class BookForManipulationDto
    {
        //putting abstract in class modifier indicates that it's only intended to be used as a base class for other derived classes

        [Required]
        [MaxLength(100, ErrorMessage = "You should fill out a title. ")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Description is required")]
        [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
        public virtual string Description { get; set; }
        //this indicates that derived classes must implement their definition of Description
        //use virtual when you have a default implementation but WANT to leave room for override in the derived classes

    }
}
