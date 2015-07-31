using System;
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

namespace Swappy_V2.Controllers
{
    public class DealsController : Controller
    {
        DataContext db = new DataContext();
        // GET: Deals
        public async Task<ActionResult> Index(string search = null, string city = null)
        {
            var ds = db.Deals.Include(p => p.Variants).Include(p => p.ItemToChange);

            if (!string.IsNullOrEmpty(search))
            {
                var res = SearchModule.FindOut(search, ds);
                var list = res.FullMatch.Concat(res.FullSubstringMatch).Concat(res.IncompleteMatch);
                var nl = from ob in list select ob.Key as DealModel;
                return View(nl.ToList());
            }
            return View(ds);
        }

        [HttpGet]
        public ActionResult Create()
        {
            DealModel deal = new DealModel
            {
                Id = new Random().Next(),
                ItemToChange = new ItemModel
                {
                    Id = new Random().Next()
                },
                Variants = new List<ItemModel> {/* new ItemView{ Id = Randomator.Next() }*/ }
            };
            return View(deal);
        }
        
        [Authorize]
        [HttpPost]
        public ActionResult Create(DealModel deal, HttpPostedFileWrapper file)
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
                    string ufile = Path.GetFileName(file.FileName);
                    string fname = ufile.Substring(ufile.LastIndexOf('.'));
                    fname = String.Format("{0}_{1}_{2}", DateTime.Now.ToString("dd.MM.yyyy - hh-mm-ss"), deal.Id, fname);
                    deal.ItemToChange.ImageUrl = Util.SaveFile(AppConstants.DealImagesPath, fname, file);
                }
                deal.AppUserId = User.Identity.GetAppUserId();
                deal.City = User.Identity.GetClaim("City");
                db.Deals.Add(deal);
                db.Items.Add(deal.ItemToChange);
                db.Items.AddRange(deal.Variants);

                db.SaveChanges();
                deal.ItemToChange.DealModelId = deal.Id;
                foreach (var i in deal.Variants)
                    i.DealModelId = deal.Id;

                db.SaveChanges();

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
           
            return View();
        }

        [HttpPost]
        public string FileValidCheck(HttpPostedFileWrapper file)
        {
            var fileValid = CustomValidator.ImageVaild(file);
            var data = new JsonObject();

            if ((fileValid & ValidStatus.Valid) == ValidStatus.Valid)
            {
                data.Status = "OK";
                string ufile = Path.GetFileName(file.FileName);
                string fname = ufile.Substring(ufile.LastIndexOf('.'));
                fname = String.Format("{0}_{1}_{2}", DateTime.Now.ToString("dd.MM.yyyy - hh-mm-ss"), new Random().Next(), fname);

                data.Result = Util.SaveFile(AppConstants.TempImagesPath, fname, file);
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
            var ud = User.Identity.GetAppUserId();
            var dls = db.Deals.Include(x => x.ItemToChange).Include(x => x.Variants).Where(x => x.AppUserId == ud).ToList();
            return View(dls);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            var ud = User.Identity.GetAppUserId();
            DealModel deal = new DealModel();
            try
            {
                deal = db.Deals.Include(x => x.ItemToChange).Include(x => x.Variants).Single(x => x.Id == id);
                // защита от несанкционированного редкатирования другим пользователем
                if (deal.AppUserId != ud)
                {
                    return RedirectToAction("MyDeals");
                }
                ViewData["EditDeal"] = true;
                return View("Create", deal);
            }
            catch
            {
                return RedirectToAction("MyDeals");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(DealModel deal, HttpPostedFileWrapper file)
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
                    string ufile = Path.GetFileName(file.FileName);
                    string fname = ufile.Substring(ufile.LastIndexOf('.'));
                    fname = String.Format("{0}_{1}_{2}", DateTime.Now.ToString("dd.MM.yyyy - hh-mm-ss"), deal.Id, fname);
                    deal.ItemToChange.ImageUrl = Util.SaveFile(AppConstants.DealImagesPath, fname, file);
                }

                DealModel oldVal = db.Deals.Include(x => x.ItemToChange).Include(x => x.Variants).Single(x => x.Id == deal.Id);
                deal.ItemToChange.ImageUrl = oldVal.ItemToChange.ImageUrl;
                db.Items.RemoveRange(oldVal.Variants);
                db.Deals.Remove(oldVal);

                deal.AppUserId = User.Identity.GetAppUserId();
                deal.City = User.Identity.GetClaim("City");
                db.Deals.Add(deal);
                db.Items.Add(deal.ItemToChange);
                db.Items.AddRange(deal.Variants);

                db.SaveChanges();
                deal.ItemToChange.DealModelId = deal.Id;
                foreach (var i in deal.Variants)
                    i.DealModelId = deal.Id;

                db.SaveChanges();

                return RedirectToAction("MyDeals");
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

            return View();
        }

        public ActionResult Info(int dealId)
        {
            var deal = db.Deals.Include(m => m.ItemToChange).Include(m => m.Variants).FirstOrDefault(m => m.Id == dealId);
            if (deal == null)
            {
                // TO DO  сделать перенаправление на страницу с ошибкой
                return HttpNotFound("Объявление не найдено");
            }
            return View(deal);
        }

        [HttpGet]
        public string GetPhoneNumber(int dealId)
        {
            var ob = new JsonObject()
            {
                Status = "ok"
            };
            if (dealId == null)
            {
                ob.Status = "Error";
                ob.Result = "dealId is null";
                return JsonConvert.SerializeObject(ob);
            }
            var deal = db.Deals.Find(dealId);

            if (deal == null)
            {
                ob.Status = "Error";
                ob.Result = "deal with id = " + dealId + " not found";
                return JsonConvert.SerializeObject(ob);
            }
            var result = db.Users.Where(x => x.Id == deal.AppUserId).First();
            ob.Result = result.PhoneNumber;
            return JsonConvert.SerializeObject(ob);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var ud = User.Identity.GetAppUserId();
            var deal = new DealModel();
            try
            {
                deal = db.Deals.Include(x => x.ItemToChange).Include(x => x.Variants).Single(x => x.Id == id);
                // защита от несанкционированного удаления другим пользователем
                if (deal.AppUserId != ud)
                {
                    return RedirectToAction("MyDeals");
                }
                return View(deal);
            }
            catch
            {
                return RedirectToAction("MyDeals");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var oldVal = db.Deals.Include(x => x.ItemToChange).Include(x => x.Variants).Single(x => x.Id == id);
            db.Items.RemoveRange(oldVal.Variants);
            db.Deals.Remove(oldVal);
            db.SaveChanges();
            return RedirectToAction("MyDeals");
        }
    }
}