using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Model
{
    public class AuthorForCreationWithDateofDeathDto : AuthorForCreationDto
    {

        public DateTimeOffset? DateOfDeath { get; set; }
    }
}
