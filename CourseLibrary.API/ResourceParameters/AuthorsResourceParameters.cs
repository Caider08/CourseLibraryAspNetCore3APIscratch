using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.ResourceParameters
{
    public class AuthorsResourceParameters
    {
        //this allows us to just pass this in as a method parameter inside controller
        // ... SO if we decide to add more parameters...we just add them inside of here
        public string MainCategory { get; set; }

        public string SearchQuery { get; set; }

    }
}
