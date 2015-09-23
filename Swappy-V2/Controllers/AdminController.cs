﻿using System;
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
        IRepository<DealModel> DealsRepo;
        IRepository<AppUserModel> UsersRepo;
        IPathProvider ServerPathProvider;
        Mockable MockHelper;

        public AdminController()
        {
            DealsRepo = new DealsRepository();
            UsersRepo = new UsersRepository();
            MockHelper = new MockingHelper();
            ServerPathProvider = new ServerPathProvider();
        }
        public AdminController(IRepository<DealModel> dealRepo, IRepository<AppUserModel> usersRepo = null, Mockable helper = null, IPathProvider pp = null)
        {
            DealsRepo = dealRepo;
            UsersRepo = usersRepo ?? new UsersRepository();
            MockHelper = helper ?? new MockingHelper();
            ServerPathProvider = pp ?? new ServerPathProvider();

        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Deals()
        {
            var list = DealsRepo.GetList();
            return View(list);
        }
        public ActionResult Users()
        {
            var users = UsersRepo.GetList();
            return View(users);
        }
        public ActionResult ShowUser(int? id)
        {
            if (id.HasValue)
            {
                var users = UsersRepo.GetList();
                try
                {
                    var user = users.Single(x => x.Id == id.Value);
                    var deals = DealsRepo.GetList().FindAll(x => x.AppUserId == id);
                    ViewBag.Deals = deals;
                    return View(user);
                }
                catch (InvalidOperationException)
                {
                    //TODO: показать ошибку, что юзер не найден
                    return RedirectToAction("Index");
                }
            }
            else
            {
                //TODO: показать ошибку, что бзер не найден
                return RedirectToAction("Index");
            }
        }
    }
}