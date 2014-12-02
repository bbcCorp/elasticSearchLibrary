using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elasticSearchLibrary.Core
{
    public interface ILibraryRepository
    {
        bool CreateLibraryIndex();
        bool DropLibraryIndex();

        List<Book> GetBooks(string criteria,int count=0);
        Task<List<Book>> GetBooksAync(string criteria, int count=0);


        List<Book> GetBooksByAuthor(string author, int count=0);
        Task<List<Book>> GetBooksByAuthorAync(string author, int count=0);
        Task<ISearchResponse<Book>> SearchBooksByAuthorAync(string author, int count=0);

        bool AddBook(Book bk);

        bool EditBook(int BookId, Book bk);

        bool RemoveBook(int BookId);

        long GetBookCount(string criteria = "");
    }
}
