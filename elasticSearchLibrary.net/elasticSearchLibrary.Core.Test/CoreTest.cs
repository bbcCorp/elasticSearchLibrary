using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace elasticSearchLibrary.Core.Test
{
    // NOTE: These tests are based on data loaded from the BookList.csv
    // If you have not seeded the index, you need to alter these tests

    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void GetBooksDefault_ExpectMax10Result()
        {
            var lib = new LibraryRepository();

            var books = lib.GetBooks();

            
            Assert.IsTrue(books.Count <= 10);
        }

        [TestMethod]
        public void GetHarryPotterBooks_ExpectMoreThan15Results()
        {
            var lib = new LibraryRepository();

            var books = lib.GetBooks("Harry Potter", 20);


            Assert.IsTrue(books.Count > 15);
        }

        [TestMethod]
        public void Get20Books_Expect20Result()
        {
            var lib = new LibraryRepository();

            var books = lib.GetBooks("",20);


            Assert.IsTrue(books.Count == 20);
        }

        [TestMethod]
        public void GetAllBooksCount_ExpectMoreThan700Results()
        {
            var lib = new LibraryRepository();

            var bookCount = lib.GetBookCount();

            Assert.IsTrue( bookCount > 700 );
        }

        [TestMethod]
        public void GetHarryPotterBooksCount_ExpectMoreThan15Results()
        {
            var lib = new LibraryRepository();

            var bookCount = lib.GetBookCount("Harry Potter");

            Assert.IsTrue(bookCount > 15);
        }

        [TestMethod]
        public void GetJKRowlingsBooks_ExpectMoreThan15Results()
        {
            var lib = new LibraryRepository();

            var books = lib.GetBooksByAuthor("J. K. Rowling", 20);


            Assert.IsTrue(books.Count > 15);
        }

    }
}
