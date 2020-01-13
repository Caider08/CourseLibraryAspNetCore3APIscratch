using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Model;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authorCollections")]
    public class AuthorsCollectionController : ControllerBase
    {

        private readonly IMapper _mapper;

        private readonly ILibraryRepository _libraryRepository;

        public AuthorsCollectionController(ILibraryRepository repo, IMapper map)
        {
            _mapper = map ?? throw new ArgumentNullException(nameof(map));

            _libraryRepository = repo ?? throw new ArgumentNullException(nameof(repo));


        }

        //array key: 1,2,3
        //compositie key : key1  = value1, ky2 = value2

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection([FromRoute] [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            //FromRoute ensures the framework gets the ids from the route (didn't have to do this in ASPnet)
            //Get Request shouldn't have a request body (where Framework looks by default)
            if(ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _libraryRepository.GetAuthors(ids);

            if(ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorsToReturn);

        }


        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(IEnumerable<AuthorForCreationDto> createAuthors)
        {
            var authorEntities = _mapper.Map<IEnumerable<Library.API.Entities.Author>>(createAuthors);

            foreach (var author in authorEntities)
            {
                _libraryRepository.AddAuthor(author);
            }

            _libraryRepository.Save();

            //since it's a POST...requires a 201 to be sent back
            //return CreatedAtRoute()
            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString }, authorCollectionToReturn);

            return Ok();

        }
    }
}
