using CourseLibrary.API.Model;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.Helpers;
using AutoMapper;
using CourseLibrary.API.ResourceParameters;
using Library.API.Entities;

namespace CourseLibrary.API.Controllers
{
    [ApiController] //not strictly neccessary
    [Route("api/authors")]
    // this is the same as above line ... [Route("api/[controller]")]
    public class AuthorsController : ControllerBase //inheriting from Controller would also enable access to views ...which we don't need for an API
    {
        private ILibraryRepository _libraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ILibraryRepository libraryRepository, IMapper mapper)
        {
            _libraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("api/authors")] //this is attribute based routing
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParams)
        {
            //[FromQuery] above isn't required unless it's a complext type which the above is
            //use ActionResult<T> whenever possible ...allows other parts of our application to make inferences

            var authorsFromRepo = _libraryRepository.GetAuthors(authorsResourceParams);

            var authors = new List<AuthorDto>();

            //this can get cumbersome ... so use an object mapper like AutoMapper wherever possible
            /*foreach(var autho in authorsFromRepo)
            {
                authors.Add(new AuthorDto()
                {
                    Id = autho.Id,
                    Name = $"{autho.FirstName} {autho.LastName}",
                    Genre = autho.Genre,
                    Age = autho.DateOfBirth.GetCurrentAge()

                }) ; 
                
               
            } */

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
            //return new JsonResult(authors); //by default returns status code 200 ... so can use return Ok(authorsFromRepo) Ok = status code 200

            
        }

        [HttpGet("{authorId}", Name ="GetAuthor")]
        //we give it a Name = so we can refer to it later (see the POST method)
        // curly brackets significy it's a parameter that may change
        //[HttpGet("{authorId:guid}")] if route can have Id or integer
        public IActionResult GetAuthor(Guid authorId)
        {
            //if need to return a resource null check it
            //if don't need to return a resource ... exist check it

            var authorFromRepo = _libraryRepository.GetAuthor(authorId);

            if(authorFromRepo == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));

        }

        [HttpPost]
        //aleady have the route at the top
        public IActionResult CreateAuthor(AuthorForCreationDto author)
        {
            /*if (author == null)
            {
                return BadRequest();
            }*/
            //above not needed...API already does it for us

            var authorEntity = _mapper.Map<Author>(author);
            _libraryRepository.AddAuthor(authorEntity);
            _libraryRepository.Save();
            //our controller should think of the dbContext like a black box...it may contain logging it may not etc...

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor",
                new { authorId = authorToReturn.Id }, authorToReturn);


        }

        [HttpOptions]
        public IActionResult GetAuthorOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {

            var authorFromRepo = _libraryRepository.GetAuthor(authorId);

            if(authorFromRepo == null)
            {
                return NotFound();
            }

            //Cascade on delete is on by default in EntityFrameWorkCore so the books for the author are deleted as well
            _libraryRepository.DeleteAuthor(authorFromRepo);

            _libraryRepository.Save();

            return NoContent();

        }

        

    }
}
