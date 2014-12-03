using elasticSearchLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace elasticSearchLibrary.Web.Controllers
{
    public class SearchController : Controller
    {
        private ILibraryRepository _repo;

        public SearchController(ILibraryRepository repository)
        {
            _repo = repository;
        }

        // GET: Search
        public ActionResult Index(string q)
        {
            ViewBag.Query = q;

            var refinements = new List<string>();
            refinements.Add("author");
            refinements.Add("genre");

            var ResultTask = _repo.SearchBookWithAggregation_Aync(q, "", refinements , 20);

            return View(ResultTask.Result);
        }
    }
}