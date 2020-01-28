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

        const int maxPageSize = 20;
        public string MainCategory { get; set; }

        public string SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize 
        { 
            
            get => _pageSize;

            set => _pageSize = (value > maxPageSize) ? _pageSize : value;
        
        }

        public string OrderBy { get; set; } = "Name";

        public string fields { get; set; }

    }
}
