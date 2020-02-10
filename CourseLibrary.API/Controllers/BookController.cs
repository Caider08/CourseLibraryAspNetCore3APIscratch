using AutoMapper;
using CourseLibrary.API.Model;
using Library.API.Entities;
using Library.API.Services;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/books")]
    //[ResponseCache(CacheProfileName = "240SecondCacheProfile")]
    //commented out because ETag middleware now takes care of it?
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate =true)]
    public class BookController : ControllerBase
    {

        private ILibraryRepository _libRepository;

        private readonly IMapper _mapper;

        public BookController(ILibraryRepository libRepository, IMapper mapper)
        {
            _libRepository = libRepository ?? throw new ArgumentNullException(nameof(libRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetBooksForAuthor")]
        [HttpHead]
        public ActionResult<IEnumerable<BookDto>> GetBooksForAuthor(Guid authorId)
        {

            if(!_libRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorFromRepo = _libRepository.GetBooksForAuthor(authorId);

            return Ok(_mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo));


        }

        [HttpGet]
        [Route("{bookId}", Name = "GetBookForAuthor")]
        //[ResponseCache(Duration = 120)]
        //need to set this method CacheLocation to public since Globally we're set to private
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate =false)]
        public ActionResult<BookDto> GetBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!_libRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libRepository.GetBookForAuthor(authorId, bookId);

            if(bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BookDto>(bookForAuthorFromRepo));
        }

        [HttpPost(Name = "CreateBookForAuthor")]
        public ActionResult<BookDto> CreateBookForAuthor(Guid authorId, BookForCreationDto book)
        {
            if (!_libRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = _mapper.Map<Library.API.Entities.Book>(book);
            _libRepository.AddBookForAuthor(authorId, bookEntity);
            _libRepository.Save();

            var bookToReturn = _mapper.Map<BookDto>(bookEntity);
            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToReturn.Id }, bookToReturn);

        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid bookid, BookForUpdateDto book)
        {
            //returning ActionResult doesn't really "add anything" anymore for our return type ... so uses IActionResult?
            if(!_libRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libRepository.GetBookForAuthor(authorId, bookid);

            if(bookForAuthorFromRepo == null)
            {
                var courseToAdd = _mapper.Map<Book>(book);
                courseToAdd.Id = bookid;

                _libRepository.AddBookForAuthor(authorId, courseToAdd);

                _libRepository.Save();

                //if we're upserting we're creating something
                var courseToReturn = _mapper.Map<BookDto>(courseToAdd);

                return CreatedAtRoute("GetBookForAuthor",
                    new { authorId = authorId, bookId = courseToReturn.Id }, courseToReturn);
            }

            //map the entity to a BookForUpdatedDto
            // apply the updated field values to that Dto
            //map the BookForUpdateDto back to an entity
            _mapper.Map(book, bookForAuthorFromRepo);

            _libRepository.UpdateBookForAuthor(bookForAuthorFromRepo);
            _libRepository.Save();
            return NoContent();

        }

        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateBookForAuthor(Guid authorId,
                Guid bookId, JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            if (!_libRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libRepository.GetBookForAuthor(authorId, bookId);

            if (bookForAuthorFromRepo == null)
            {
                var bookDto = new BookForUpdateDto();
                patchDocument.ApplyTo(bookDto, ModelState);

                if(!TryValidateModel(bookDto))
                {
                    return ValidationProblem(ModelState);
                }
                var bookToAdd = _mapper.Map<Library.API.Entities.Book>(bookDto);
                bookToAdd.Id = bookId;

                _libRepository.AddBookForAuthor(authorId, bookToAdd);
                _libRepository.Save();

                var bookToReturn = _mapper.Map < BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor", new { authorId, bookId = bookToReturn.Id },
                    bookToReturn);
            }

            var bookToPatch = _mapper.Map<BookForUpdateDto>(bookForAuthorFromRepo);

            //add validation
            patchDocument.ApplyTo(bookToPatch, ModelState);

            if(!TryValidateModel(bookToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(bookToPatch, bookForAuthorFromRepo);

            _libRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            _libRepository.Save();

            return NoContent();


        }

        [HttpDelete("{bookId}")]
        
        public ActionResult DeleteBookForAuthor(Guid authorId, Guid bookId)
        {
            if(!_libRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libRepository.GetBookForAuthor(authorId, bookId);

            if(bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            _libRepository.DeleteBook(bookForAuthorFromRepo);

            _libRepository.Save();

            //successful but nothing to return since it was deleted
            return NoContent();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return base.ValidationProblem(modelStateDictionary);
        }

    }
}
