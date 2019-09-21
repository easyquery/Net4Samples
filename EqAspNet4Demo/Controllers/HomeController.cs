using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EqAspNet4Demo.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("privacy")]
        public ActionResult Privacy()
        {
            return View();
        }

        [Route("advanced-search")]
        public ActionResult AdvancedSearch()
        {
            return View();
        }

        [Authorize]
        [Route("adhoc-reporting")]
        public ActionResult AdHocReporting()
        {
            return View();
        }
    }
}