using AutoMapper;
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
    [Route("api/authors/{authorId}/books")]
    public class BookController : ControllerBase
    {

        private ILibraryRepository _libRepository;

        private readonly IMapper _mapper;

        public BookController(ILibraryRepository libRepository, IMapper mapper)
        {
            _libRepository = libRepository ?? throw new ArgumentNullException(nameof(libRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
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

        [HttpPost]
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
        public ActionResult UpdateBookForAuthor(Guid authorId, Guid bookid, BookForUpdateDto book)
        {
            if(!_libRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libRepository.GetBookForAuthor(authorId, bookid);

            if(bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            //map the entity to a BookForUpdatedDto
            // apply the updated field values to that Dto
            //map the BookForUpdateDto back to an entity
            _mapper.Map(book, bookForAuthorFromRepo);

            _libRepository.UpdateBookForAuthor(bookForAuthorFromRepo);
            _libRepository.Save();
            return NoContent();

        }

    }
}
