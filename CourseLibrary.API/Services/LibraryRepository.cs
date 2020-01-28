using CourseLibrary.API.Helpers;
using CourseLibrary.API.Model;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Library.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext _context;

        private IPropertyMappingService _propertyMappingService;

        public LibraryRepository(LibraryContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            //Entity framework will actually do this for us
            author.Id = Guid.NewGuid();
            

            // the repository fills the id (instead of using identity columns)
            if (author.Books.Any())
            {
                foreach (var book in author.Books)
                {
                    book.Id = Guid.NewGuid();
                }
            }

            _context.Authors.Add(author);
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author != null)
            {
                // if there isn't an id filled out (ie: we're not upserting),
                // we should generate one
                if (book.Id == Guid.Empty)
                {
                    book.Id = Guid.NewGuid();
                }
                author.Books.Add(book);
            }
        }

        public bool AuthorExists(Guid authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Author GetAuthor(Guid authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParams)
        {
            if(authorsResourceParams == null)
            {
                throw new ArgumentNullException(nameof(authorsResourceParams));
            }
            
            //Don't want this now since we have additional parameters with the page# and page size blah blah blah
            /*if(string.IsNullOrWhiteSpace(authorsResourceParams.MainCategory) && string.IsNullOrWhiteSpace(authorsResourceParams.SearchQuery))
            {
                return GetAuthors();
            }*/

            var collection = _context.Authors as IQueryable<Author>;
            //allows us to use Where clauses etc.. on the collection

            if (!string.IsNullOrWhiteSpace(authorsResourceParams.MainCategory))
            {

                var mainCategory = authorsResourceParams.MainCategory.Trim();
                collection = collection.Where(a => a.Genre == mainCategory);
            }

            if(!string.IsNullOrWhiteSpace(authorsResourceParams.SearchQuery))
            {
                var searchQuery = authorsResourceParams.SearchQuery.Trim();
                collection = collection.Where(a => a.Genre.Contains(searchQuery)
                    || a.FirstName.Contains(searchQuery)
                    || a.LastName.Contains(searchQuery));
                
            }

            if(!string.IsNullOrWhiteSpace(authorsResourceParams.OrderBy))
            {
               /* if(authorsResourceParams.OrderBy.ToLowerInvariant() == "name")
                {
                    collection = collection.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
                }*/

                //get property mapping dictionary
                var authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<AuthorDto, Author>();

                collection = collection.ApplySort(authorsResourceParams.OrderBy, authorPropertyMappingDictionary);
                
            }




            //nothing affects the Db performance wise until we get here?
            return PagedList<Author>.Create(collection, authorsResourceParams.PageNumber, authorsResourceParams.PageSize);
            
            /*return collection
                .Skip(authorsResourceParams.PageSize * (authorsResourceParams.PageNumber -1)) //how much we need to skip
                .Take(authorsResourceParams.PageSize) //how much we take per page
                .ToList();*/
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return _context.Books
              .Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return _context.Books
                        .Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }

        public void UpdateBookForAuthor(Book book)
        {
            // no code in this implementation
            // _mapper.Map() and then doing context.save() is doing it for us

        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
