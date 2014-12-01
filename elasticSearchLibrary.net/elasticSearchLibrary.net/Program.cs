using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using elasticSearchLibrary.Core;
using CsvHelper;
using System.IO;
using System.Diagnostics;

namespace elasticSearchLibrary.net
{

    public class BookReader
    {
        public string ISBN;
        public string Title;
        public string Author;
        public string Genre;
    }

    class Program
    {
        private static ElasticSearchHelper esHelper;

        static void Main(string[] args)
        {
            
            esHelper = new ElasticSearchHelper();

            // If you do not have a library index, use this function to create and initiate the library index
            //CreateIndexAndAddSomeBooks();
            
            // Use this to seed the index with some books from the file BookList.csv
            //UploadFromBookList();

            SearchDemo();

            Console.WriteLine("Press any key to exit ... ");
            Console.ReadKey();
        }

        /// <summary>
        /// Call this function if you want to create a new default index and a few books to the index
        /// </summary>
        static void CreateIndexAndAddSomeBooks()
        {
            Console.WriteLine("Let's create an index called: library ");

            // Use this method to create a new index called "library"
            esHelper.CreateIndex("library");

            Console.WriteLine("Let's index a few books ... ");

            var bk1 = new Book()
            {
                ContentId = "ISBN 978-0-307-27812-8",
                Author = "Brian Greene",
                Title = "The Hidden Reality",
                Genre = "Popular Science",
                PublishDate = new DateTime(2011, 1, 1)
            };


            var bk2 = new Book()
            {
                ContentId = "ISBN-13: 9781451675047",
                Author = "Richard Dawkins, Dave McKean",
                Title = "The Magic of Reality: How We Know What's Really True",
                Genre = "Science - General & Miscellaneous",
                PublishDate = new DateTime(2012, 9, 11)
            };

            var bk3 = new Book()
            {
                ContentId = "ISBN-13: 9780062225795",
                Author = "Richard Dawkins",
                Title = "An Appetite for Wonder: The Making of a Scientist",
                Genre = "Biography",
                PublishDate = new DateTime(2013, 9, 24)
            };

            esHelper.IndexBook(bk1);
            esHelper.IndexBook(bk2);
            esHelper.IndexBook(bk3);

        }

        static void SearchDemo()
        {
            Console.WriteLine("Now we try out some searches... ");

            //Now we search
            String searchForWord = String.Empty;

            Console.WriteLine("Demo of full text search. Enter the keyword to search for. ");
            searchForWord = Console.ReadLine();
            var result = esHelper.SearchForWordsMatching(searchForWord);


            Console.WriteLine("Enter the author to search for: ");
            searchForWord = Console.ReadLine();
            result = esHelper.SearchForWordsMatching(searchForWord, "author");
        }

        static void UploadFromBookList()
        {
            Console.WriteLine("Let's index a few books ... ");

            List<Book> listOfBooks = new List<Book>();

            var fileReader = new StreamReader(File.OpenRead(@"..\..\files\BookList.csv"));

            while (!fileReader.EndOfStream)
            {
                var line = fileReader.ReadLine();
                var values = line.Split(',');

                if(values.Count() >=4 )
                {
                    var book = new Book { Author = values[2], ContentId = "ISBN " + values[0], Genre = values[3], Title = values[1] };

                    listOfBooks.Add(book);
                    
                }
                

            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var book in listOfBooks)
            {
                esHelper.IndexBook(book);
            }

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00} hr :{1:00} min :{2:00}.{3:00} sec", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine("{0} books added in {1}.", listOfBooks.Count, elapsedTime);
        }
    }
}
