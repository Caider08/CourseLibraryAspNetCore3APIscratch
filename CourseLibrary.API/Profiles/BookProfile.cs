using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Library.API.Entities.Book, Model.BookDto>();

            CreateMap<Model.BookForCreationDto, Library.API.Entities.Book > ();

            CreateMap<Model.BookForUpdateDto, Library.API.Entities.Book>();

            CreateMap<Library.API.Entities.Book, Model.BookForUpdateDto>();
        }
        

    }
}
