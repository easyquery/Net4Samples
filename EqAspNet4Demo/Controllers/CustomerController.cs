using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using X.PagedList;

using Korzh.EasyQuery.Linq;

using EqAspNet4Demo.Models;

namespace EqAspNet4Demo.Controllers
{
    [RoutePrefix("fulltext-search")]
    public class CustomerController: Controller
    {

        private readonly ApplicationDbContext _dbContext;

        public CustomerController()
        {
            _dbContext = new ApplicationDbContext();
        }


        [Route("")]
        public ActionResult Index(string text, int? page)
        {
            var result = (!string.IsNullOrEmpty(text))
                        ? _dbContext.Customers.FullTextSearchQuery(text)
                        : _dbContext.Customers;

            if (!page.HasValue)
                page = 1;

            var pageNumber = page ?? 1;
    
            ViewBag.PageOfCustomers = result.OrderBy(c => c.Id).ToPagedList(pageNumber, 25);
            ViewBag.Text = text;

            return View("Customers");
        }
    }
}