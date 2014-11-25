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
            
            // Use this method to create a new index
            //esHelper.CreateIndex();

            esHelper.InsertData();


            Console.WriteLine("Press any key to exit ... ");
            Console.ReadKey();
        }
    }
}
