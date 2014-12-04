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


        bool AddBook(Book bk);

        bool EditBook(int BookId, Book bk);

        bool RemoveBook(int BookId);

        long GetBookCount(string criteria = "");


        List<Book> GetBooks(string criteria,int count=0);
        Task<List<Book>> GetBooksAync(string criteria, int count=0);


        List<Book> GetBooksByAuthor(string author, int count=0);
        Task<List<Book>> GetBooksByAuthorAync(string author, int count=0);
        Task<ISearchResponse<Book>> SearchBooksByAuthorAync(string author, int count=0);

        Task<ISearchResponse<Book>> ElasticSearch_Book_Aync(string criteria = "", string searchField = "", int count = 10);

        Task<ISearchResponse<Book>> SearchBookWithAggregation_Aync(string criteria = "", string searchField = "", List<string> refinements = null, int count = 10);

        ISearchResponse<Book> SearchBookWithAggregation(string criteria = "", string searchField = "", List<string> refinements = null, int count = 10);
        ISearchResponse<Book> SearchBookWithAggregationFilters(string criteria = "", string searchField = "", List<string> refinements = null, Dictionary<string, string> filters = null, int count = 10);
        

    }
}
