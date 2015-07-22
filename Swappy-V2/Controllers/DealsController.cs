using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Swappy_V2.Controllers
{
    public class DealsController : Controller
    {
        // GET: Deals
        public ActionResult Index()
        {
            return View();
        }
    }
}