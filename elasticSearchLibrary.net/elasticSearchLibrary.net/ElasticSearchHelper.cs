using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using System.Configuration;
using System.Net;

namespace elasticSearchLibrary.net
{
    // This is a basic helper class to explore elasticsearch engine and the Nest library.
    // Created by: Bedabrata Chatterjee - Nov 2014   
    public class ElasticSearchHelper
    {
        private Uri esNode;
        private ConnectionSettings esConnection;
        private ElasticClient esClient ;

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

        public void CreateIndex()
        {
            try
            {
                var esIndexSettings = new IndexSettings();
                esIndexSettings.NumberOfShards = 5;
                esIndexSettings.NumberOfReplicas = 1;

                esClient.CreateIndex(c => c.Index("library")
                                            .InitializeUsing(esIndexSettings)
                                            .AddMapping<Book>(m => m.MapFromAttributes())
                                    );

                Console.WriteLine("library Index has been created.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating index. Message: {0}", ex.Message);
            }

            

        }


        public void InsertData()
        {
            var bk1 = new Book() {  ContentId = "ISBN 978-0-307-27812-8" , 
                                    Author = "Brian Greene", 
                                    Title = "The Hidden Reality", 
                                    Genre = "Popular Science", 
                                    PublishDate = new DateTime(2011, 1, 1) 
             };

            esClient.Index(bk1);

            var bk2 = new Book()    { 
                                        ContentId = "ISBN-13: 9781451675047", 
                                        Author = "Richard Dawkins, Dave McKean", 
                                        Title = "The Magic of Reality: How We Know What's Really True", 
                                        Genre = "Science - General & Miscellaneous", 
                                        PublishDate = new DateTime(2012, 9, 11) 
            };
            esClient.Index(bk2);

            var bk3 = new Book()
            {
                ContentId = "ISBN-13: 9780062225795",
                Author = "Richard Dawkins",
                Title = "An Appetite for Wonder: The Making of a Scientist",
                Genre = "Biography", 
                        PublishDate = new DateTime(2013, 9, 24) 
            };
            esClient.Index(bk3);

            Console.WriteLine("Books added to library index ");
        }
    }
}
