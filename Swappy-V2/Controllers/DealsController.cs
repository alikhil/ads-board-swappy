using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Swappy_V2.Models;

namespace Swappy_V2.Controllers
{
    public class DealsController : Controller
    {
        // GET: Deals
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(DealModel deal, HttpPostedFileWrapper file)
        {
            return View();
        }
    }
}