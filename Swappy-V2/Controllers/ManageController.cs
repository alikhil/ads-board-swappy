﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Data.Entity;
using Swappy_V2.Models;
using Swappy_V2.Classes;
using System.Security.Claims;

namespace Swappy_V2.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        IRepository<DealModel> DealsRepo;
        IRepository<AppUserModel> UsersRepo;
        IPathProvider ServerPathProvider;
        Mockable MockHelper;

        public ManageController() : this(null, null, null, null)
        {
           
        }
        public ManageController(IRepository<DealModel> dealRepo = null, IRepository<AppUserModel> usersRepo = null, Mockable helper = null, IPathProvider pp = null)
        {
            DealsRepo = dealRepo ?? new DealsRepository();
            UsersRepo = usersRepo ?? new UsersRepository();
            MockHelper = helper ?? new MockingHelper();
            ServerPathProvider = pp ?? new ServerPathProvider();
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ваш пароль изменен."
                : message == ManageMessageId.SetPasswordSuccess ? "Пароль задан."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Настроен поставщик двухфакторной проверки подлинности."
                : message == ManageMessageId.Error ? "Произошла ошибка."
                : message == ManageMessageId.AddPhoneSuccess ? "Ваш номер телефона добавлен."
                : message == ManageMessageId.RemovePhoneSuccess ? "Ваш номер телефона удален."
                : "";

            var userId = MockHelper.GetUserId(User.Identity);
            var appUserId = MockHelper.GetAppUserId(User.Identity);
            var appUser = UsersRepo.GetAll().SingleOrDefault(x => x.Id == appUserId);
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                City = appUser.City,
                Name = appUser.Name,
                PhoneNumber = appUser.PhoneNumber,
                Surname = appUser.Surname
            };
            await Task.FromResult(0);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAppModelData(IndexViewModel model)
        {
            bool cityIsValid = await CustomValidator.CityValid(model.City);

            if (ModelState.IsValid && cityIsValid)
            {
                var appUserId = MockHelper.GetAppUserId(User.Identity);
                var appUser = UsersRepo.GetAll().SingleOrDefault(x => x.Id == appUserId);
                //Обновление профиля в своей таблице
                appUser.Name = model.Name;
                appUser.PhoneNumber = model.PhoneNumber;
                appUser.Surname = model.Surname;
                appUser.City = model.City;

                UsersRepo.Update(appUser);
                UsersRepo.Save();
                //В таблице юзеров(?Зачем хранить дубликаты?)
                //TODO: Подумать над этим вопросом

                var user = UserManager.Users.FirstOrDefault(x => x.AppUserId == appUserId);
                user.City = model.City;
                user.Surname = model.Surname;
                user.PhoneNumber = model.PhoneNumber;
                user.Name = model.Name;

                UpdateClaims(model);

                await UserManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            if (!cityIsValid)
                ModelState.AddModelError("City", "Указанный город не существует");
            return View("Index", model);
        }

        private void UpdateClaims(IndexViewModel model)
        {
            var AuthenticationManager = HttpContext.GetOwinContext().Authentication;
            var Identity = new ClaimsIdentity(User.Identity);

            Identity.RemoveClaim(Identity.FindFirst("Name"));
            Identity.AddClaim(new Claim("Name", model.Name));

            Identity.RemoveClaim(Identity.FindFirst("City"));
            Identity.AddClaim(new Claim("City", model.City));

            Identity.RemoveClaim(Identity.FindFirst("Surname"));
            Identity.AddClaim(new Claim("Surname", model.Surname));

            AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(
                new ClaimsPrincipal(Identity), new AuthenticationProperties { IsPersistent = true });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Запрос перенаправления к внешнему поставщику входа для связывания имени входа текущего пользователя
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}