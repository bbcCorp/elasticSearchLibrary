using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using elasticSearchLibrary.Core;
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
        private static ILibraryRepository repo;

        static void Main(string[] args)
        { 
            repo = new LibraryRepository();

            // If you do not have a library index, use this function to create and initiate the library index
            CreateIndexAndAddSomeBooks();
            
            // Use this to seed the index with some books from the file BookList.csv
            UploadFromBookList();

            SearchDemo();

        }

        /// <summary>
        /// Call this function if you want to create a new default index and a few books to the index
        /// </summary>
        static void CreateIndexAndAddSomeBooks()
        {
            Console.WriteLine("Let's create an index called: library ");

            // Use this method to create a new index called "library"
            if (repo.CreateLibraryIndex())
            {
                Console.WriteLine("Index has been created.");
            }

            Console.WriteLine("Let's index a few books ... ");

            var bk1 = new Book()
            {
                Id = 1,
                ContentId = "ISBN 978-0-307-27812-8",
                Author = "Brian Greene",
                Title = "The Hidden Reality",
                Genre = "Popular Science",
                PublishDate = new DateTime(2011, 1, 1)
            };


            var bk2 = new Book()
            {
                Id = 2,
                ContentId = "ISBN-13: 9781451675047",
                Author = "Richard Dawkins, Dave McKean",
                Title = "The Magic of Reality: How We Know What's Really True",
                Genre = "Science - General & Miscellaneous",
                PublishDate = new DateTime(2012, 9, 11)
            };

            var bk3 = new Book()
            {
                Id = 3,
                ContentId = "ISBN-13: 9780062225795",
                Author = "Richard Dawkins",
                Title = "An Appetite for Wonder: The Making of a Scientist",
                Genre = "Biography",
                PublishDate = new DateTime(2013, 9, 24)
            };

            repo.AddBook(bk1);
            repo.AddBook(bk2);
            repo.AddBook(bk3);

        }

        static void SearchDemo()
        {
            Console.WriteLine("Now we try out some searches... ");

            //Now we search
            String searchForWord = String.Empty;



            Console.WriteLine("Enter the author to search for: ");
            searchForWord = Console.ReadLine().Trim().ToLower();

            foreach (var book in repo.GetBooksByAuthor(searchForWord, 20))
            {
                Console.WriteLine("\n * Book Title: {0} - By {1} *", book.Title, book.Author);
            }
            Console.WriteLine("----------------------------------");

            Console.WriteLine("Demo of full text search. Enter the keyword to search for. Type quit to exit");
            Console.WriteLine("\nSearch > ");
            searchForWord = Console.ReadLine().Trim().ToLower();

            if (searchForWord.Equals("quit"))
                return;

            do
            {
                foreach (var book in repo.GetBooks(searchForWord,20))
                {
                    Console.WriteLine("\n * Book Title: {0} - By {1} *", book.Title, book.Author);
                }
                Console.WriteLine("----------------------------------");
                Console.WriteLine("\nSearch > ");
                searchForWord = Console.ReadLine();
            }
            while (!searchForWord.Trim().ToLower().Equals("quit"));



        }

        /// <summary>
        /// Function to seed library index with books from files\BookList.csv
        /// </summary>
        static void UploadFromBookList()
        {
            Console.WriteLine("Let's index a few books ... ");

            List<Book> listOfBooks = new List<Book>();

            var fileReader = new StreamReader(File.OpenRead(@"..\..\files\BookList.csv"));

            int startIndex = 5;

            while (!fileReader.EndOfStream)
            {
                var values = parseCSV(fileReader.ReadLine());

                if(values.Count() >=4 )
                {
                    var book = new Book { Id=startIndex++, Author = values[2].Trim(), ContentId = "ISBN " + values[0].Trim(), Genre = values[3].Trim(), Title = values[1].Trim() };

                    listOfBooks.Add(book);
                    
                }
                

            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var book in listOfBooks)
            {
                if(! repo.AddBook(book))
                {
                    Console.WriteLine("Book was already present in index. Record updated");
                }
            }

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00} hr :{1:00} min :{2:00}.{3:00} sec", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine("{0} books added in {1}.", listOfBooks.Count, elapsedTime);
        }

        /// <summary>
        ///  A simple function to parse a CSV line and return the string tokens
        ///  This function also takes care of tokens with comma enclosed in double quotes
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        static List<String> parseCSV(string csv)
        {
            List<String> tokens = new List<string>();

            int counter = 0;
            int iBegin = 0;
            int iEnd = 0;

            bool inString = false;
            bool escapeFollowingQuote = false;

            if (csv[csv.Length - 1] != ',')
                csv = csv + ',';

            while (counter < csv.Length)
            {
                // Condition 1: Regular comma 
                if ((csv[counter] == ',') && (inString == false))
                {
                    iEnd = counter;

                    if (iBegin != iEnd)
                    {
                        string tokenToAdd = csv.Substring(iBegin, iEnd - iBegin).Trim();

                        if (tokenToAdd.StartsWith("\"") && tokenToAdd.EndsWith("\"") && tokenToAdd.Length > 2)
                            tokenToAdd = tokenToAdd.Substring(1, tokenToAdd.Length - 2);

                        tokens.Add(tokenToAdd);
                    }
                    else
                        tokens.Add("");

                    counter++;
                    iBegin = counter;
                    iEnd = counter;

                    continue;
                }

                // Condition 2: Handling start of a double quoted string 
                if ((csv[counter] == '"'))
                {
                    if (inString == false)
                    {
                        inString = true;
                        escapeFollowingQuote = false;


                        iBegin = counter;
                        iEnd = counter;

                        counter++;
                        continue;
                    }
                    else
                    {
                        // Condition 3: Within a double quoted string, handle escape sequence of double quotes 
                        if (escapeFollowingQuote == true)
                        {
                            escapeFollowingQuote = false;
                        }
                        else
                        {
                            if (((counter + 1) < csv.Length) && (csv[counter + 1] == '"'))
                                escapeFollowingQuote = true;

                            if (((counter + 1) < csv.Length) && (csv[counter + 1] == ','))
                            {
                                inString = false;
                            }


                        }

                        counter++;
                        continue;

                    }
                }

                counter++;
            }

            return tokens;
        }
    }
}
