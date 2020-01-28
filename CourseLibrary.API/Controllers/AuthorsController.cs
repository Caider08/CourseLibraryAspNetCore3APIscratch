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
using System.Text.Json;
using CourseLibrary.API.Services;

namespace CourseLibrary.API.Controllers
{
    [ApiController] //not strictly neccessary
    [Route("api/authors")]
    // this is the same as above line ... [Route("api/[controller]")]
    public class AuthorsController : ControllerBase //inheriting from Controller would also enable access to views ...which we don't need for an API
    {
        private ILibraryRepository _libraryRepository;
        private  IMapper _mapper;
        private IPropertyMappingService _propertyMappingService;
        private IPropertyCheckerService _ipropertyCheckerService;


        public AuthorsController(ILibraryRepository libraryRepository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyCheckerService checkerService)
        {
            _libraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));

            _ipropertyCheckerService = checkerService ?? throw new ArgumentNullException(nameof(checkerService));


        }

        //[HttpGet("api/authors")] //this is attribute based routing
        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        public IActionResult GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParams)
        {
            //[FromQuery] above isn't required unless it's a complext type which the above is
            //use ActionResult<T> whenever possible ...allows other parts of our application to make inferences

            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParams.OrderBy))
            {
                return BadRequest();
            }

            if (!_ipropertyCheckerService.TypehasProperties<AuthorDto>(authorsResourceParams.fields))
            {
                return BadRequest();
            }

            var authorsFromRepo = _libraryRepository.GetAuthors(authorsResourceParams);

            //var authors = new List<AuthorDto>();

            /*var previouspageLink = authorsFromRepo.HasPrevious ?
                    CreateAuthorsResourceUri(authorsResourceParams,
                    ResourceUriType.PreviousPage) : null;

            var nextPageLink = authorsFromRepo.HasNext ?
                CreateAuthorsResourceUri(authorsResourceParams,
                ResourceUriType.NextPage) : null; */

            var paginationMetaData = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            var links = CreateLinksForAuthors(authorsResourceParams, authorsFromRepo.HasNext, authorsFromRepo.HasPrevious);

            var shapedAuthors = _mapper.Map<IEnumerable < AuthorDto >> (authorsFromRepo)
                .ShapeData(authorsResourceParams.fields);

            var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
            {
                var authorsAsDictionary = author as IDictionary<string, object>;
                var authorLinks = CreateLinksForAuthor((Guid)authorsAsDictionary["Id"], null);
                authorsAsDictionary.Add("links", authorLinks);
                return authorsAsDictionary;
            });

            var linksCollectionResource = new
            {
                value = shapedAuthorsWithLinks,
                links
            };

            return Ok(linksCollectionResource);

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

            //return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo)
                //.ShapeData(authorsResourceParams.fields));
            //return new JsonResult(authors); //by default returns status code 200 ... so can use return Ok(authorsFromRepo) Ok = status code 200

            
        }

        [HttpGet("{authorId}", Name ="GetAuthor")]
        //we give it a Name = so we can refer to it later (see the POST method)
        // curly brackets significy it's a parameter that may change
        //[HttpGet("{authorId:guid}")] if route can have Id or integer
        public IActionResult GetAuthor(Guid authorId, string fields)
        {
            if(!_ipropertyCheckerService.TypehasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }
            //if need to return a resource null check it
            //if don't need to return a resource ... exist check it

            var authorFromRepo = _libraryRepository.GetAuthor(authorId);

            if(authorFromRepo == null)
            {
                return NotFound();
            }

            var links = CreateLinksForAuthor(authorId, fields);

            var linkedResourceToReturn =
                _mapper.Map<AuthorDto>(authorFromRepo).ShapeData(fields)
                    as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);

           //return Ok(_mapper.Map<AuthorDto>(authorFromRepo).ShapeData(fields));

        }

        [HttpPost(Name = "CreateAuthor")]
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

            var links = CreateLinksForAuthor(authorToReturn.Id, null);

            var linkedResourceToReturn = authorToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor",
                new { authorId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);


        }

        [HttpOptions]
        public IActionResult GetAuthorOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }

        [HttpDelete("{authorId}", Name ="Delete Author")]
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

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
        {

            switch(type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery,
                        });

                case ResourceUriType.Current:
                    

                default:
                    return Url.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery,
                        });
            }


        }

        private IEnumerable<LinkDto> CreateLinksForAuthor(Guid authorId, string fields)
        {
            var links = new List<LinkDto>();

            if(string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetAuthor", new { authorId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("Get Author", new { authorId, fields }),
                    "self",
                    "GET"));
            }

            links.Add(
                new LinkDto(Url.Link("Delete Author", new { authorId }),
                "delete_author",
                "DELETE"));

            links.Add(
                new LinkDto(Url.Link("CreateBookForAuthor", new { authorId }),
                "create_book_for_author",
                "POST"));

            links.Add(
                new LinkDto(Url.Link("GetBooksForAuthor", new { authorId }),
                "books",
                "GET"));

            

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(
            AuthorsResourceParameters authorsResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            //self
            links.Add(
                new LinkDto(CreateAuthorsResourceUri(
                    authorsResourceParameters, ResourceUriType.Current),
                    "self", "GET"));

            if(hasNext)
            {
                links.Add(
                    new LinkDto(CreateAuthorsResourceUri(
                        authorsResourceParameters, ResourceUriType.NextPage),
                        "nextPage", "GET"));
            }

            if(hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateAuthorsResourceUri(
                        authorsResourceParameters, ResourceUriType.PreviousPage),
                        "previousPage", "GET"));
            }
            return links;
        }

    }
}
