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
            var dls = db.Deals.Include(x => x.Variants);
            var list = dls.ToList();
            return View(list);
        }
        public ActionResult Users()
        {
            var users = db.Users.ToList();
            return View(users);
        }
        public ActionResult ShowUser(int? id)
        {
            if (id.HasValue)
            {
                var users = db.Users.ToList();
                var user = users.Single(x => x.Id == id);
                var deals = db.Deals.Include(x => x.Variants).Where(x => x.AppUserId == id);
                ViewBag.Deals = deals.ToList();
                return View(user);
            }
            else
            {
                //TODO: показать ошибку, что бзер не найден
                return RedirectToAction("Index");
            }
        }
    }
}