using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.Model;
using CourseLibrary.API.Helpers;
using Library.API.Entities;

namespace CourseLibrary.API.Profiles
{
    public class AuthorsProfile : Profile
    { 

        public AuthorsProfile()
        {
            CreateMap<Library.API.Entities.Author, Model.AuthorDto>()
                    .ForMember(
                            dest => dest.Name,
                            opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                    .ForMember(
                             dest => dest.Age,
                             opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

            CreateMap<Model.AuthorForCreationDto, Library.API.Entities.Author > ();

            CreateMap<Model.AuthorForCreationWithDateofDeathDto, Library.API.Entities.Author>();

            CreateMap<Author, Model.AuthorFullDto>();

        }


    }
}
