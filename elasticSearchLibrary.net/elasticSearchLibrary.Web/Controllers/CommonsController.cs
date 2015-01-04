using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace elasticSearchLibrary.Web.Controllers
{
    public class CommonsController : Controller
    {
        // GET: Commons
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HttpStatus404()
        {
            return View();
        }
    }
}