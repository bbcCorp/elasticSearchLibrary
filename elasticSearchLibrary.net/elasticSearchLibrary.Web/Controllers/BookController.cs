﻿using elasticSearchLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace elasticSearchLibrary.Web.Controllers
{
    public class BookController : Controller
    {
        private ILibraryRepository _repo;

        public BookController(ILibraryRepository repository)
        {
            _repo = repository;
        }


        // GET: Book
        public ActionResult Index()
        {
            var books = _repo.GetBooks("", 10);
            return View(books);
        }

        // GET: Book/Details/5
        public ActionResult Details(string id)
        {
            var book = _repo.GetBook(id);
            return View(book);
        }

        // GET: Book/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        [HttpPost]
        public ActionResult Create(Book bk)
        {
            try
            {
                // TODO: Add insert logic here
                if(_repo.AddBook(bk))
                    return RedirectToAction("Index");

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Book/Edit/5
        public ActionResult Edit(int id)
        {
            var book = _repo.GetBookByID(id);

            if (book == null)
                return RedirectToAction("HttpStatus404");

            return View(book);

        }

        // POST: Book/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Book bk)
        {
            try
            {
                // TODO: Add update logic here
                if(_repo.EditBook(id, bk))
                    return RedirectToAction("Index");

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Book/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
