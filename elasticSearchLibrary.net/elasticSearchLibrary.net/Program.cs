using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elasticSearchLibrary.net
{
 
    class Program
    {
        static void Main(string[] args)
        {
            var esHelper = new ElasticSearchHelper();
            
            // Use this method to create a new index called "library"
            //esHelper.CreateIndex();

            // Now add a few books to the "library" index
            //esHelper.InsertData();

            //Now we search
            String searchForWord = String.Empty;

            Console.WriteLine("Enter the keyword to search for: ");
            searchForWord = Console.ReadLine();
            var result = esHelper.SearchForWordsMatching(searchForWord);


            Console.WriteLine("Enter the author to search for: ");
            searchForWord = Console.ReadLine();
            result = esHelper.SearchForWordsMatching(searchForWord, "author");

            Console.WriteLine("Press any key to exit ... ");
            Console.ReadKey();
        }
    }
}
