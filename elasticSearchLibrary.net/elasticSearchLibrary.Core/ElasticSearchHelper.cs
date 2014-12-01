using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using System.Configuration;
using System.Net;

namespace elasticSearchLibrary.Core
{
    // This is a basic helper class to explore elasticsearch engine and the Nest library.
    // Created by: Bedabrata Chatterjee - Nov 2014   
    // For more information on the NEST library - Refer to http://nest.azurewebsites.net/nest/quick-start.html
    public class ElasticSearchHelper
    {
        private Uri esNode;
        private ConnectionSettings esConnection;
        private ElasticClient esClient;

        private Boolean TestNode(String uri)
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

        public ElasticSearchHelper()
        {
            var uri = ConfigurationManager.AppSettings["NestNodeUri"];

            if (!TestNode(uri))
            {
                throw new ApplicationException("Error: Please check the Nest Node settings. Bad uri or elasticsearch Server is not responding");
            }

            esNode = new Uri(uri);
            esConnection = new ConnectionSettings(esNode, defaultIndex: "library");

            esClient = new ElasticClient(esConnection);
        }

        /// <summary>
        /// Use this function to create a new Elasticsearch index
        /// </summary>
        /// <param name="indexName"></param>
        public bool CreateIndex(String indexName)
        {
            bool retVal = false;

            try
            {
                var tCreateIndex = Task.Factory.StartNew(() =>
                {
                    var esIndexSettings = new IndexSettings();
                    esIndexSettings.NumberOfShards = 1;
                    esIndexSettings.NumberOfReplicas = 1;

                    return esClient.CreateIndex(c => c.Index(indexName)
                                                .InitializeUsing(esIndexSettings)
                                                .AddMapping<Book>(m => m.MapFromAttributes())
                                        );

                });

                var result = tCreateIndex.Result;
                if (result.Acknowledged == true)
                {
                    Console.WriteLine("library Index has been created.");
                }

                return result.Acknowledged;
            }
            catch (AggregateException ae)
            {

                foreach (Exception e in ae.Flatten().InnerExceptions)
                {
                    Console.WriteLine("Error creating index. Message: {0}", e.Message);
                }
                return retVal;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating index. Message: {0}", ex.Message);
            }
            return retVal;
        }

        public void IndexBook(Book bk)
        {
            try
            {
                if (bk != null)
                {
                    var tIndexBook = Task.Factory.StartNew(() =>
                    {

                        return esClient.Index(bk);
                    });

                    var result = tIndexBook.Result;

                    if (result.Created == true)
                    {
                        Console.WriteLine("Book has been created and added to index");
                    }
                    else
                    {
                        Console.WriteLine("Book was already present. Updated and re-indexed the book. Document version: {0}", result.Version );
                    }

                }



            }
            catch (AggregateException ae)
            {

                foreach (Exception e in ae.Flatten().InnerExceptions)
                {
                    Console.WriteLine("Indexing error. Message: {0}", e.Message);
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in adding book. Message:{0}", ex.Message);
                throw;
            }

        }


        private void SeedBooks()
        {

            try
            {
                var tAddBook1 = Task.Factory.StartNew(() =>
                {

                    var bk1 = new Book()
                    {
                        ContentId = "ISBN 978-0-307-27812-8",
                        Author = "Brian Greene",
                        Title = "The Hidden Reality",
                        Genre = "Popular Science",
                        PublishDate = new DateTime(2011, 1, 1)
                    };

                    esClient.Index(bk1);

                });

                var tAddBook2 = Task.Factory.StartNew(() =>
                {

                    var bk2 = new Book()
                    {
                        ContentId = "ISBN-13: 9781451675047",
                        Author = "Richard Dawkins, Dave McKean",
                        Title = "The Magic of Reality: How We Know What's Really True",
                        Genre = "Science - General & Miscellaneous",
                        PublishDate = new DateTime(2012, 9, 11)
                    };
                    esClient.Index(bk2);

                });

                var tAddBook3 = Task.Factory.StartNew(() =>
                {

                    var bk3 = new Book()
                    {
                        ContentId = "ISBN-13: 9780062225795",
                        Author = "Richard Dawkins",
                        Title = "An Appetite for Wonder: The Making of a Scientist",
                        Genre = "Biography",
                        PublishDate = new DateTime(2013, 9, 24)
                    };
                    esClient.Index(bk3);

                });

                Task.WaitAll(new Task[] { tAddBook1, tAddBook2, tAddBook3 });


                Console.WriteLine("Books added to library index ");
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("Error in seed function");

                foreach (Exception e in ae.Flatten().InnerExceptions)
                {
                    Console.WriteLine("Error Message: {0}", e.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in seed function. Message:{0}", ex.Message);
            }


        }

        /// <summary>
        /// This function does a full text search for matchWord in the searchField. If searchField is not mentioned, it searches title field
        /// </summary>
        /// <param name="matchWord"></param>
        /// <param name="searchField"></param>
        /// <returns></returns>
        public List<Book> SearchForWordsMatching(string matchWord, string searchField = "_all")
        {
            if (String.IsNullOrEmpty(matchWord))
            {
                Console.WriteLine("You have to enter a keyword to search for");
                throw new ApplicationException("Keyword not supplied for search operation");
            }

            if(searchField != "_all")
                Console.WriteLine("Limiting search to field: {0}", searchField);

            List<Book> results = null;
            try
            {
                Task<ISearchResponse<Book>> tSearch = Task.Factory.StartNew(() =>
                {
                    var queryResult = esClient.Search<Book>(es => es.Query(
                                p => p.Match(m => m.OnField(searchField).Query(matchWord))
                                ));


                    return queryResult;
                });

                tSearch.Wait();

                

                Console.WriteLine("Number of matches for the word: '{0}' is: {1}", matchWord, tSearch.Result.Total);

                results = tSearch.Result.Documents.ToList<Book>();

                if (results != null && results.Count() > 0)
                {
                    Console.WriteLine("Displaying Top {0} book(s).", results.Count());
                    foreach (var book in results)
                    {
                        Console.WriteLine("\n * Book Title: {0} - By {1} *", book.Title, book.Author);
                    }
                }

                Console.WriteLine("----------------------------------");

                return results;
            }
            catch (AggregateException ae)
            {

                foreach (Exception e in ae.Flatten().InnerExceptions)
                {
                    Console.WriteLine("Search error. Message: {0}", e.Message);
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Search error. Message: {0}", ex.Message);
                throw;
            }
        }
    }
}
