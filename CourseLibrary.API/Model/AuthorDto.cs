using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Model
{
    public class AuthorDto
    {
        //the one returned by our Controller Action ... when we don't want to just return entire record from DB
   
        public Guid Id { get; set; }

    
        public string Name { get; set; }

           
        public int Age { get; set; }

       
        public string Genre { get; set; }


    }
}
