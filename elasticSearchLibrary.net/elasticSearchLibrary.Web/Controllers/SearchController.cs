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
        public ActionResult Index()
        {
            return View();
        }

        // GET: Search
        public ActionResult Result(string q, string filter)
        {
            ViewBag.Query = q;

            var refinements = new List<string>();
            refinements.Add("author");
            refinements.Add("genre");


            Dictionary<string, string> SearchFilters = null;

            if (!String.IsNullOrEmpty(filter))
            {
                var filterArray = HttpUtility.UrlDecode(filter).Split(new char[] { ':' });
                if(filterArray.Count() == 2)
                {
                    SearchFilters = new Dictionary<string, string>();
                    SearchFilters.Add(filterArray[0], filterArray[1]);
                }
            }

            
            var ResultTask = _repo.SearchBookWithAggregationFilters(q, "", refinements, SearchFilters, 20);
            return View(ResultTask);

        }
    }
}