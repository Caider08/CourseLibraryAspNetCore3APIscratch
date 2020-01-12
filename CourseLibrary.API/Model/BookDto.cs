using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Model
{
    public class BookDto
    {
       
        public Guid Id { get; set; }

     
        public string Title { get; set; }

   
        public string Description { get; set; }

        //this would hurt performance below 
        //public AuthorDto Author { get; set; }

        public Guid AuthorId { get; set; }
    }
}
