﻿using elasticSearchLibrary.Core.Model;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace elasticSearchLibrary.Core
{
    public class LibraryRepository : ILibraryRepository
    {
        private Uri esNode;
        private ConnectionSettings esConnection;
        private ElasticClient esClient;

        private const String C_INDEXNAME = "library";
        private const String C_BOKTYPENAME = "book";

        public LibraryRepository()
        {
            try
            {
                InitiateElasticSearchClient();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("Error connecting to ElasticSearchNode: {0}", ex.Message));
            }
        }

        private void InitiateElasticSearchClient()
        {
            var uri = ConfigurationManager.AppSettings["NestNodeUri"];

            if (!TestElasticSearchNode(uri))
            {
                throw new ApplicationException("Error: Please check the Nest Node settings. Bad uri or elasticsearch Server is not responding");
            }

            esNode = new Uri(uri);
            esConnection = new ConnectionSettings(esNode, defaultIndex: "library");

            esClient = new ElasticClient(esConnection);
        }

        private Boolean TestElasticSearchNode(String uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Verified connection to elasticSearch node");
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Use this function to create a new Elasticsearch index
        /// </summary>
        /// <param name="C_INDEXNAME"></param>
        public bool CreateLibraryIndex()
        {
            try
            {
                var tCreateIndex = Task.Factory.StartNew(() =>
                {
                    var esIndexSettings = new IndexSettings();
                    esIndexSettings.NumberOfShards = 1;
                    esIndexSettings.NumberOfReplicas = 1;

                    return esClient.CreateIndex(c => c.Index(C_INDEXNAME)
                                                .InitializeUsing(esIndexSettings)
                                                .AddMapping<Book>(m => m.MapFromAttributes())
                                        );

                });

                var result = tCreateIndex.Result;

                return result.Acknowledged;
            }
            catch (AggregateException ae)
            {
                StringBuilder exceptions = new StringBuilder();

                foreach (Exception e in ae.Flatten().InnerExceptions)
                {
                    exceptions.Append(String.Format("\n Error creating index. Message: {0}", e.Message));
                }

                throw new ApplicationException(String.Format("Error creating index. Message: {0}", exceptions.ToString()));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("Error creating index. Message: {0}", ex.Message));
            }

        }


        public bool DropLibraryIndex()
        {
            var result = esClient.DeleteIndex(C_INDEXNAME);

            return result.Acknowledged;
        }

        public Task<ISearchResponse<Book>> ElasticSearch_Book_Aync(string criteria = "", string searchField = "", int count = 10)
        {
            Task<ISearchResponse<Book>> tSearch = Task.Factory.StartNew(() =>
            {
                ISearchResponse<Book> queryResult;

                if (String.IsNullOrEmpty(criteria))
                {
                    queryResult = esClient.Search<Book>(es => es.From(0).Size(count).Type("book").MatchAll());

                }
                else
                {
                    if (String.IsNullOrEmpty(searchField))
                        searchField = "_all";

                    queryResult = esClient.Search<Book>(es => es.From(0).Size(count).Type("book")
                                                    .Query(q => q.Match(m => m.OnField(searchField).Query(criteria))));
                }


                return queryResult;
            });

            return tSearch;

        }

        public ISearchResponse<Book> SearchBookWithAggregation(string criteria = "", string searchField = "", List<string> refinements = null, int count = 10)
        {
            QueryContainer _query;

            if (String.IsNullOrEmpty(criteria))
            {
                _query = new MatchAllQuery();
            }
            else
            {
                if (String.IsNullOrEmpty(searchField))
                {
                    searchField = "_all";

                    _query = new MatchQuery()
                    {
                        Field = searchField,
                        Query = criteria
                    };
                }
                else
                {
                    _query = new TermQuery()
                    {
                        Field = searchField,
                        Value = criteria
                    };
                }

            }

            SearchRequest searchRequest = new SearchRequest
            {
                
                From = 0,
                Size = count,
                Query = _query
            };

            if (refinements != null && refinements.Count > 0)
            {
                var _aggregations = new Dictionary<string, IAggregationContainer>();

                foreach (var field in refinements)
                {
                    _aggregations.Add(field, new AggregationContainer
                    {
                        Terms = new TermsAggregator
                        {
                            Field = field
                        }
                    });
                }

                searchRequest.Aggregations = _aggregations;

            }

            return esClient.Search<Book>(searchRequest);
        }

        public Task<ISearchResponse<Book>> SearchBookWithAggregation_Aync(string criteria = "", string searchField = "", List<string> refinements = null, int count = 10)
        {
            Task<ISearchResponse<Book>> tSearch = Task.Factory.StartNew(() =>
            {

                QueryContainer _query;

                if (String.IsNullOrEmpty(criteria))
                {
                    _query = new MatchAllQuery();
                }
                else
                {
                    if (String.IsNullOrEmpty(searchField))
                    {
                        searchField = "_all";

                        _query = new MatchQuery()
                        {
                            Field = searchField,
                            Query = criteria
                        };
                    }
                    else
                    {
                        _query = new TermQuery()
                        {
                            Field = searchField,
                            Value = criteria
                        };
                    }

                }

                SearchRequest searchRequest = new SearchRequest
                {
                    From = 0,
                    Size = count,
                    Query = _query
                };

                if (refinements != null && refinements.Count > 0)
                {
                    var _aggregations = new Dictionary<string, IAggregationContainer>();

                    foreach (var field in refinements)
                    {
                        _aggregations.Add(field, new AggregationContainer
                        {
                            Terms = new TermsAggregator
                            {
                                Field = field                               
                            }
                        });
                    }

                    searchRequest.Aggregations = _aggregations;

                }

                var qResult = esClient.Search<Book>(searchRequest);

                return qResult;
            });

            return tSearch;

        }

        public ISearchResponse<Book> SearchBookWithAggregationFilters(string criteria = "", string searchField = "", 
            List<string> refinements = null, 
            Dictionary<string,string> SearchFilters = null, 
            int count = 10)
        {
            QueryContainer _query;

            if (String.IsNullOrEmpty(criteria))
            {
                _query = new MatchAllQuery();
            }
            else
            {
                if (String.IsNullOrEmpty(searchField))
                {
                    searchField = "_all";

                    _query = new MatchQuery()
                    {
                        Field = searchField,
                        Query = criteria
                    };
                }
                else
                {
                    _query = new TermQuery()
                    {
                        Field = searchField,
                        Value = criteria
                    };
                }

            }

            SearchRequest searchRequest = new SearchRequest
            {

                From = 0,
                Size = count,
                Query = _query
            };

            if (refinements != null && refinements.Count > 0)
            {
                var _aggregations = new Dictionary<string, IAggregationContainer>();

                foreach (var field in refinements)
                {
                    _aggregations.Add(field, new AggregationContainer
                    {
                        Terms = new TermsAggregator
                        {
                            Field = field
                        }
                    });
                }

                searchRequest.Aggregations = _aggregations;

            }

            if (SearchFilters != null && SearchFilters.Count > 0)
            {
                var searchFilterConfig = new FilterContainer();

                foreach(var sf in SearchFilters)
                {
                    searchFilterConfig = Filter<Book>.Term( sf.Key, sf.Value );
                }

                searchRequest.Filter = searchFilterConfig;
            }

            return esClient.Search<Book>(searchRequest);
        }

        /// <summary>
        /// Method to get list of books that matches defined criteria
        /// </summary>
        /// <param name="count">Optional. Default is 10 result</param>
        /// <param name="criteria">Optional. Pass full text search criteria</param>
        /// <returns></returns>
        public List<Book> GetBooks(string criteria = "", int count = 10)
        {
            var tResult = GetBooksAync(criteria, count);

            return tResult.Result;
        }

        public Book GetBook(string id)
        {
            Task<ISearchResponse<Book>> tBookSearch = Task.Factory.StartNew<ISearchResponse<Book>>((BookId) =>
            {

                String _id = (string)BookId;

                return esClient.Search<Book>(es => es.Type(C_BOKTYPENAME).Query(q => q.Term(b => b.OnField(f => f.ContentId).Value(_id))));

            },id);

            if (tBookSearch.Result != null && tBookSearch.Result.Total == 1)
                return tBookSearch.Result.Documents.First<Book>();
            else
                return null;
        }


        public Book GetBookByID(int id)
        {
            Task<ISearchResponse<Book>> tBookSearch = Task.Factory.StartNew<ISearchResponse<Book>>((BookId) =>
            {

                int _id = (int) BookId;

                return esClient.Search<Book>(es => es.Type(C_BOKTYPENAME).Query(q => q.Term(b => b.OnField(f => f.Id).Value(_id))));

            }, id);

            if (tBookSearch.Result != null && tBookSearch.Result.Total == 1)
                return tBookSearch.Result.Documents.First<Book>();
            else
                return null;
        }

        /// <summary>
        /// Async method to get list of books that matches defined criteria
        /// </summary>
        /// <param name="count">Optional. Default is 10 result</param>
        /// <param name="criteria">Optional. Pass full text search criteria</param>
        /// <returns></returns>
        public Task<List<Book>> GetBooksAync(string criteria = "", int count = 10)
        {
            Task<List<Book>> tSearch = Task.Factory.StartNew(() =>
            {
                return ElasticSearch_Book_Aync(criteria, "", count).Result.Documents.ToList<Book>();
            });

            return tSearch;

        }

        public ISearchResponse<Book> MultiMatchANDSearch(List<AdvancedSearchFilter> SearchCriteria, List<string> refinements = null, Dictionary<string, string> SearchFilters = null, int count = 10)
        {
            QueryContainer _query = null;

            foreach(var rec in SearchCriteria)
            {
                if ((!String.IsNullOrEmpty(rec.SearchField)) && (!String.IsNullOrEmpty(rec.SearchField)))
                {
                    var qry = new MatchQuery()
                    {
                        Field = rec.SearchField,
                        Query = rec.SearchQuery
                    };

                    if (_query == null)
                        _query = qry;
                    else
                        _query = _query && qry;
                }
            }

            SearchRequest searchRequest = new SearchRequest
            {

                From = 0,
                Size = count,
                Query = _query
            };

            if (refinements != null && refinements.Count > 0)
            {
                var _aggregations = new Dictionary<string, IAggregationContainer>();

                foreach (var field in refinements)
                {
                    _aggregations.Add(field, new AggregationContainer
                    {
                        Terms = new TermsAggregator
                        {
                            Field = field
                        }
                    });
                }

                searchRequest.Aggregations = _aggregations;

            }

            if (SearchFilters != null && SearchFilters.Count > 0)
            {
                var searchFilterConfig = new FilterContainer();

                foreach (var sf in SearchFilters)
                {
                    searchFilterConfig = Filter<Book>.Term(sf.Key, sf.Value);
                }

                searchRequest.Filter = searchFilterConfig;
            }

            return esClient.Search<Book>(searchRequest);
        }


        /// <summary>
        /// This API is similar to GetBooksAync except that it returns the raw ISearchResponse object
        /// instead of a list of books
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="count"></param>
        /// <returns>Task<ISearchResponse<Book>></returns>
        public Task<ISearchResponse<Book>> SearchBooksAync(string criteria = "", int count = 10)
        {
            return ElasticSearch_Book_Aync(criteria, "", count);

        }

        public List<Book> GetBooksByAuthor(string author, int count = 10)
        {
            var tResult = GetBooksByAuthorAync(author, count);

            return tResult.Result;
        }

        public Task<List<Book>> GetBooksByAuthorAync(string author, int count = 10)
        {
            Task<List<Book>> tSearch = Task.Factory.StartNew(() =>
            {
                return ElasticSearch_Book_Aync(author, "author", count).Result.Documents.ToList<Book>();
            });

            return tSearch;
        }

        /// <summary>
        /// This API is similar to GetBooksByAuthorAync except that it returns the raw ISearchResponse object
        /// instead of a list of books
        /// </summary>
        /// <param name="author"></param>
        /// <param name="count"></param>
        /// <returns>Task<ISearchResponse<Book>></returns>
        public Task<ISearchResponse<Book>> SearchBooksByAuthorAync(string author, int count = 10)
        {

            return ElasticSearch_Book_Aync(author, "author", count);
        }

        public long GetBookCount(string criteria = "")
        {
            try
            {
                Task<ISearchResponse<Book>> tSearch = Task.Factory.StartNew(() =>
                {
                    return ElasticSearch_Book_Aync(criteria, "", 10).Result;
                });

                tSearch.Wait();

                return tSearch.Result.Total;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("Error while retrieving count : {0}", ex.Message));
            }
        }

        /// <summary>
        /// This API will return true if the book was created
        /// and false if an existing book was updated
        /// </summary>
        /// <param name="bk"></param>
        /// <returns></returns>
        public Boolean AddBook(Book bk)
        {
            try
            {
                var result = AddBookAsync(bk).Result;

                return result.Created;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format("Error indexing book : {0}", ex.Message));
            }


        }

        public Task<IIndexResponse> AddBookAsync(Book bk)
        {
            var tIndexBook = Task.Factory.StartNew(() =>
            {
                return esClient.Index(bk, i=>i.Index(C_INDEXNAME)
                                              .Type(C_BOKTYPENAME)
                                              .Id(bk.Id));
            });

            return tIndexBook;
        }

        public Boolean EditBook(int BookId, Book bk)
        {
            var status =  esClient.Index(bk, i => i.Index(C_INDEXNAME)
                                             .Type(C_BOKTYPENAME)
                                             .Id(BookId));
            return true;
        }

        public Boolean RemoveBook(int BookId)
        {
            return true;
        }
    }
}
