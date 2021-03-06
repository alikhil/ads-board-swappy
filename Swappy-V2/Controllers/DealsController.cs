﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Swappy_V2.Models;
using Swappy_V2.Classes;
using Swappy_V2.Modules;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Swappy_V2.Modules.DealMatchingModule;

namespace Swappy_V2.Controllers
{
    public class DealsController : Controller
    {
        IRepository<DealModel> DealsRepo;
        IRepository<AppUserModel> UsersRepo;
        IPathProvider ServerPathProvider;
        Mockable MockHelper;
        public DealsController()
        {
            DealsRepo = new DealsRepository();
            UsersRepo = new UsersRepository();
            MockHelper = new MockingHelper();
            ServerPathProvider = new ServerPathProvider();
        }
        public DealsController(IRepository<DealModel> dealRepo, IRepository<AppUserModel> usersRepo = null, Mockable helper = null, IPathProvider pp = null)
        {
            DealsRepo = dealRepo;
            UsersRepo = usersRepo ?? new UsersRepository();
            MockHelper = helper ?? new MockingHelper();
            ServerPathProvider = pp ?? new ServerPathProvider();

        }
        
        public ActionResult Index(string search = null, string city = null)
        {
            var deals = DealsRepo.GetAll();

            if (!string.IsNullOrEmpty(search))
            {
                var res = SearchModule.FindOut(search, deals);
                var list = res.FullMatch.Concat(res.FullSubstringMatch).Concat(res.IncompleteMatch);
                var nl = from ob in list select ob.Key as DealModel;
                return View(nl.ToList());
            }
            return View(deals.ToList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            DealModel deal = new DealModel
            {
                Id = new Random().Next(),
                Variants = new List<ItemModel> {/* new ItemView{ Id = Randomator.Next() }*/ }
            };
            return View(deal);
        }
        
        [Authorize]
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Price,AnotherVariants,ImageUrl,Description,Title,Variants")]DealModel deal, HttpPostedFileBase file)
        {
            bool fValid = file != null;
            JsonObject res = new JsonObject { Status = "OK", Result = new List<string>() };
            ValidStatus validStatus = ValidStatus.Unknown;
            if (fValid)
            {
                validStatus = CustomValidator.ImageVaild(file);
                fValid = validStatus == ValidStatus.Valid ? fValid : false;
            }

            if (ModelState.IsValid && (file == null || fValid))
            {
                if (fValid)
                {

                    deal.ImageUrl = UploadImage(deal, file);
                    deal.Images = new List<ImageModel>();
                    deal.Images.Add(
                        new ImageModel() 
                        { 
                            Deal = deal, 
                            Url = deal.ImageUrl 
                        });
                }
                deal.AppUserId = MockHelper.GetAppUserId(User.Identity);
                deal.City = MockHelper.GetClaim(User.Identity, "City");
                
                foreach (var i in deal.Variants)
                    i.DealModel = deal;
                DealsRepo.Create(deal);
                DealsRepo.Save();
                DealMatchingModule.Instance.FindMatch(deal);
                return RedirectToAction("Index");
            }
            else
            {
                if (file != null)
                {
                    List<string> errors = GenerateErrors(validStatus);
                    foreach (var error in errors)
                        ModelState.AddModelError("file", error);
                }
            }
           
            return View(deal);
        }

        [HttpPost]
        public string FileValidCheck(HttpPostedFileBase file)
        {
            var fileValid = CustomValidator.ImageVaild(file);
            var data = new JsonObject();

            if ((fileValid & ValidStatus.Valid) == ValidStatus.Valid)
            {
                data.Status = "OK";
                string ufile = Path.GetFileName(file.FileName);
                string fname = ufile.Substring(ufile.LastIndexOf('.'));
                fname = String.Format("{0}_{1}_{2}", DateTime.Now.ToString("dd.MM.yyyy - hh-mm-ss"), new Random().Next(), fname);

                data.Result = Util.SaveFile(AppConstants.TempImagesPath, fname, file, ServerPathProvider);
            }
            else
            {
                data.Result = GenerateErrors(fileValid); ;
                data.Status = "Error";
            }
            return JsonConvert.SerializeObject(data);
        }

        private static List<string> GenerateErrors(ValidStatus fileValid)
        {
            var list = new List<string>();

            if ((fileValid & ValidStatus.MaxLengthOverload) == ValidStatus.MaxLengthOverload)
                list.Add(String.Format("Превышен максимальный допустимы размер {0} МБ", AppConstants.MaxImageLengthBytes / 1024 / 1024));

            if ((fileValid & ValidStatus.IncorrectType) == ValidStatus.IncorrectType)
                list.Add("Выбранный файл должен быть изображением");

            if ((fileValid & ValidStatus.IncorrectFormat) == ValidStatus.IncorrectFormat)
                list.Add(String.Format("Изображение должно быть в формате {0}", String.Join(",", AppConstants.AllowedImageExtensions)));
            return list;
        }

        public ActionResult AddNewItem()
        {
            ItemModel item = new ItemModel { Id = new Random().Next(), Title = "", Description = "" };
            ViewData["color"] = new Random().Next() % 2 == 1 ? "success" : "primary";
            ViewData["Edit"] = true;
            return PartialView("ShowItem", item);
        }

        public string CheckItemValid(ItemModel item)
        {
            if (ModelState.IsValid)
                return "ok";
            return "*" + string.Join("<br/>*", from key in ModelState.Keys.SelectMany(key => this.ModelState[key].Errors) select key.ErrorMessage);
        }


        [Authorize]
        public ActionResult MyDeals()
        {
            var ud = MockHelper.GetAppUserId(User.Identity);
            var list = DealsRepo.GetAll().Where(x => x.AppUserId == ud).ToList();
            return View(list);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            var ud = MockHelper.GetAppUserId(User.Identity);
            DealModel deal = new DealModel();
            try
            {
                var list = DealsRepo.GetAll();
                deal = list.Single(x => x.Id == id);
                // защита от несанкционированного редкатирования другим пользователем
                if (deal.AppUserId != ud && !(User.IsInRole("Moderator") || User.IsInRole("Admin")))
                    throw new Exception("Access denied - 403. Попытка редактировать чужое объявление");

                ViewData["EditDeal"] = true;
                return View("Create", deal);
            }
            catch(Exception e)
            {
                //TODO: log Exceptoion
                return RedirectToAction("MyDeals");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Price,AnotherVariants,ImageUrl,Description,Title,Variants")]DealModel deal, HttpPostedFileBase file)
        {
            bool fValid = file != null;
            JsonObject res = new JsonObject { Status = "OK", Result = new List<string>() };
            ValidStatus validStatus = ValidStatus.Unknown;

            if (fValid)
            {
                validStatus = CustomValidator.ImageVaild(file);
                fValid = validStatus == ValidStatus.Valid ? fValid : false;
            }

            if(ModelState.IsValid)
            {
                try
                {
                    var oldVal = DealsRepo.GetAll().SingleOrDefault(x => x.Id == deal.Id);
                    // Проверка прав доступа
                    if(MockHelper.GetAppUserId(User.Identity) != oldVal.AppUserId && !(User.IsInRole("Admin") || User.IsInRole("Moderator")))
                        throw new Exception("Access denied - 403. Попытка редактировать чужое объявление");

                    if (file == null || fValid)
                    {
                        if (fValid)
                        {
                            oldVal.ImageUrl = UploadImage(deal, file);
                            
                            if (oldVal.Images == null || oldVal.Images.Count == 0)
                            {
                                oldVal.Images = oldVal.Images ?? new List<ImageModel>();
                                oldVal.Images.Add(
                                   new ImageModel()
                                   {
                                       Deal = oldVal,
                                       Url = oldVal.ImageUrl
                                   });
                            }
                            else
                            {
                                var images = oldVal.Images.ToList();
                                images[0].Url = oldVal.ImageUrl;
                                images[0].Deal = oldVal;
                                oldVal.Images = images;
                            }
                        }

                        oldVal.WithDescription(deal.Description)
                            .WithTitle(deal.Title)
                            .WithVariants(deal.Variants)
                            .WithAnotherVariants(deal.AnotherVariants)
                            .WithPrice(deal.Price);

                        foreach (var item in oldVal.Variants)
                            item.DealModel = oldVal;

                        DealsRepo.Update(oldVal);
                        DealsRepo.Save();
                        DealMatchingModule.Instance.FindMatch(oldVal);
                        return RedirectToAction("MyDeals");
                    }

                }
                catch(Exception e)
                {
                    //TODO: log exception
                    ModelState.AddModelError("Deal", e.Message);
                }

            }
           
            if (!fValid && file != null)
            {
                List<string> errors = GenerateErrors(validStatus);
                foreach (var error in errors)
                    ModelState.AddModelError("file", error);
            }

            return View("Create", deal);
        }

        private string UploadImage(DealModel deal, HttpPostedFileBase file)
        {
            string ufile = Path.GetFileName(file.FileName);
            string fname = ufile.Substring(ufile.LastIndexOf('.'));
            fname = String.Format("{0}_{1}_{2}", DateTime.Now.ToString("dd.MM.yyyy - hh-mm-ss"), deal.Id, fname);
            return Util.SaveFile(AppConstants.DealImagesPath, fname, file, ServerPathProvider);
        }

        public ActionResult Info(int dealId)
        {
            var deal = DealsRepo.GetAll().FirstOrDefault(m => m.Id == dealId);
            if (deal == null)
            {
                // TO DO  сделать перенаправление на страницу с ошибкой
                return HttpNotFound("Объявление не найдено");
            }
            return View(deal);
        }

        [HttpGet]
        public string GetPhoneNumber(int dealId = -1)
        {
            var ob = new JsonObject()
            {
                Status = "ok"
            };
            if (dealId == -1)
            {
                ob.Status = "Error";
                ob.Result = "dealId is null";
                return JsonConvert.SerializeObject(ob);
            }
            var deal = DealsRepo
                .GetAll()
                .FirstOrDefault(x => x.Id == dealId);

            if (deal == null)
            {
                ob.Status = "Error";
                ob.Result = "deal with id = " + dealId + " not found";
                return JsonConvert.SerializeObject(ob);
            }
            var result = UsersRepo
                .GetAll()
                .FirstOrDefault(x => x.Id == deal.AppUserId);
            ob.Result = result.PhoneNumber;
            return JsonConvert.SerializeObject(ob);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var ud = MockHelper.GetAppUserId(User.Identity);
            var deal = new DealModel();
            try
            {
                deal = DealsRepo
                    .GetAll()
                    .Single(x => x.Id == id);
                // защита от несанкционированного удаления другим пользователем кроме админа ;)
                if (deal.AppUserId != ud && !User.IsInRole("Admin"))
                    throw new Exception("Access denied - 403. Попытка удалить чужое объявление");
                
                return View("Delete", deal);
            }
            catch(Exception e)
            {
                //TODO: Log exception
                return RedirectToAction("MyDeals");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var oldVal = DealsRepo
                    .GetAll()
                    .Single(x => x.Id == id);
                
                if (oldVal.AppUserId != MockHelper.GetAppUserId(User.Identity) && !User.IsInRole("Admin"))
                    throw new Exception("Access denied - 403. Попытка удалить чужое объявление");

                DealsRepo.Delete(id);
                DealsRepo.Save();
            }
            catch(Exception e)
            {
                //TODO: Log exception
            }
            return RedirectToAction("MyDeals");
        }
    }
}