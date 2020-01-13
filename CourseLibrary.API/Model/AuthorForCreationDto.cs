using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Model
{
    public class AuthorForCreationDto
    {

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string MainCategory { get; set; }

        public ICollection<BookForCreationDto> Books { get; set; }
            = new List<BookForCreationDto>();
    }
}
