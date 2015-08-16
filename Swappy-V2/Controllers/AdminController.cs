using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Swappy_V2.Classes;
using Swappy_V2.Models;

namespace Swappy_V2.Controllers
{
    [AdminFilter]
    public class AdminController : Controller
    {
        DataContext db = new DataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Deals()
        {
            var dls = db.Deals.Include(x => x.ItemToChange).Include(x => x.Variants);
            var list = dls.ToList();
            return View(list);
        }
    }
}