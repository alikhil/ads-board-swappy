using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using Newtonsoft.Json;
using Swappy_V2.Models;
using Swappy_V2.Controllers;
using Swappy_V2.Classes;
using System.Security.Claims;
using Swappy_V2.Tests.Generators;
namespace Swappy_V2.Tests.Controllers
{
    [TestClass]
    public class DealsControllerTest
    {
        FakesGenerator MoqGenerator;
        public DealsControllerTest()
        {
            MoqGenerator = new FakesGenerator();
        }

        [TestMethod]
        public void DealsIndexSearching_FindResults()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            DealsController dealsController = new DealsController(dealMoq.Object);
            int searchingDealId = 0;
            string search = deals[searchingDealId].Title;
            //Act
            ViewResult result = dealsController.Index(search) as ViewResult;
            var searchResult = (List<DealModel>)result.Model;
            //Assert
            Assert.IsTrue(searchResult.Contains(deals[searchingDealId]));
        }

        /// <summary>
        /// Проверка на то, чтобы все объявления показывались
        /// </summary>
        [TestMethod]
        public void DealsIndexShowingDeals()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            DealsController dealsController = new DealsController(dealMoq.Object);
            //Act
            ViewResult result = dealsController.Index() as ViewResult;
            var searchResult = (List<DealModel>)result.Model;
            //Assert
            CollectionAssert.AreEqual(searchResult, deals);
        }

        [TestMethod]
        public void DealsCreateView_Exist()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            DealsController dealsController = new DealsController(dealMoq.Object);
            //Act
            ViewResult result = dealsController.Create() as ViewResult;
            var model = (DealModel)result.Model;
            //Assert
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void DealsCreatePostWithoutFile_NotValid()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            DealsController dealsController = new DealsController(dealMoq.Object);
            var deal = new DealModel();

            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;

            //Act
            ViewResult result = dealsController.Create(deal, null) as ViewResult;
            //Assert
            Assert.IsFalse(dealsController.ModelState.IsValid);
        }

        [TestMethod]
        public void DealsCreatePostWithFile_NotValidBoth()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var fileMoq = MoqGenerator.GetNotValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            DealsController dealsController = new DealsController(dealMoq.Object);
            var deal = new DealModel();

            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;

            //Act
            ViewResult result = dealsController.Create(deal, fileMoq.Object) as ViewResult;
            var errorList = result.ViewData.ModelState["file"].Errors.ToList();
            //Assert
            Assert.IsTrue(errorList.Count > 0);
            Assert.IsFalse(dealsController.ModelState.IsValid);
        }

        [TestMethod]
        public void DealsCreatePostWithFile_NotValidOnlyFile()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var fileMoq = MoqGenerator.GetNotValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            DealsController dealsController = new DealsController(dealMoq.Object);
            var deal = deals[0];

            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;

            //Act
            ViewResult result = dealsController.Create(deal, fileMoq.Object) as ViewResult;
            var errorList = result.ViewData.ModelState["file"].Errors.ToList();
            //Assert
            Assert.IsTrue(errorList.Count > 0);
        }

        [TestMethod]
        public void DealsCreatePostWithFile_NotValidOnlyModel()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var fileMoq = MoqGenerator.GetValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            DealsController dealsController = new DealsController(dealMoq.Object);
            var deal = new DealModel();
            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;
            
            //Act
            ViewResult result = dealsController.Create(deal, fileMoq.Object) as ViewResult;
            var fileError = result.ViewData.ModelState["file"];
            //Assert
            Assert.IsNull(fileError);
            Assert.IsFalse(dealsController.ModelState.IsValid);
        }

        [TestMethod]
        public void DealsCreatePostWithFile_ValidBoth()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();

            var fileMoq = MoqGenerator.GetValidFile();

            //инициализация мока помощника
            var moqHelper = MoqGenerator.GetMoqHelper();

            //списки для тестовых данных
            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();
            
            //генерация тестовых данных
            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            // отлавливаем вызов функций получения списков и возвращаем свои тестовые списки
            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            
            // мок для подмены путей сервера
            var pathProviderMoq = MoqGenerator.GetServerPathProvider();
            
            //подделка-мок для User.Identity
            var controllerContext = MoqGenerator.GetControllerWithContextUserIdentity();
            
            //Создание самого контроллера
            DealsController dealsController = new DealsController(dealMoq.Object, pp: pathProviderMoq.Object)
            {
                //задаем подделанный мок, для User.Identity
                ControllerContext = controllerContext.Object
            };

            var deal = deals[0];
            //Генерирум ошибки валидации для модели, потому что фунция 
            //в контроллере этого не сделат из-за того что функция вызвана не из представления
            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;
           
            //Act
            //вызов функции
            ViewResult result = dealsController.Create(deal, fileMoq.Object) as ViewResult;
            //Assert
            //проверка на валидность
            dealMoq.Verify(x => x.Create(It.IsAny<DealModel>()));
            dealMoq.Verify(x => x.Save());
            Assert.IsTrue(dealsController.ModelState.IsValid);
        }

        [TestMethod]
        public void DealsFileValidCheck_Valid()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            var fileMoq = MoqGenerator.GetValidFile();
            var pathProvider = MoqGenerator.GetServerPathProvider();

            DealsController dealsController = new DealsController(dealMoq.Object, pp: pathProvider.Object);
            //Act
            var result = dealsController.FileValidCheck(fileMoq.Object);
            var obj = JsonConvert.DeserializeObject<JsonObject>(result);
            
            //Assert
            Assert.AreEqual(obj.Status, "OK");
        }

        [TestMethod]
        public void DealsFileValidCheck_NotValid()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            var fileMoq = MoqGenerator.GetNotValidFile();
            var pathProvider = MoqGenerator.GetServerPathProvider();

            DealsController dealsController = new DealsController(dealMoq.Object, pp: pathProvider.Object);
            //Act
            var result = dealsController.FileValidCheck(fileMoq.Object);
            var obj = JsonConvert.DeserializeObject<JsonObject>(result);

            //Assert
            Assert.AreNotEqual(obj.Status, "OK");
        }

        [TestMethod]
        public void DealsAddNewItemView_Exist()
        {
            //Arrange
            DealsController dealsController = new DealsController();
            //Act
            var result = dealsController.AddNewItem() as PartialViewResult;
            var model = result.Model;
            //Assert
            Assert.IsNotNull(model);
            Assert.IsTrue(result.ViewName == "ShowItem");
        }

        [TestMethod]
        public void DealsCheckItemValid_NotValid()
        {
            //Arrange
            ItemModel item = new ItemModel();
            DealsController dealsController = new DealsController();
            dealsController = MoqGenerator.GenerateModelErrors<ItemModel>(dealsController, item) as DealsController;

            //Act
            var result = dealsController.CheckItemValid(item);
            //Assert
            Assert.IsTrue(result != "ok");
        }

        [TestMethod]
        public void DealsCheckItemValid_Valid()
        {
            //Arrange
            ItemModel item = new ItemModel()
            {
                Title = "Title",
                Id = 1,
                Description = "True discr"
            };
            DealsController dealsController = new DealsController();
            dealsController = MoqGenerator.GenerateModelErrors<ItemModel>(dealsController, item) as DealsController;

            //Act
            var result = dealsController.CheckItemValid(item);
            //Assert
            Assert.IsTrue(result == "ok");
        }

        [TestMethod]
        public void DealsMyDeals_ShowDeals()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var dealCntPerUser = 2;
            //инициализация мока помощника
            var moqHelper = MoqGenerator.GetMoqHelper();

            //списки для тестовых данных
            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            //генерация тестовых данных
            var gen = new Generators.DataGenerator();
            gen.Generate(3, dealCntPerUser, 3, out users, out deals, out items);

            // отлавливаем вызов функций получения списков и возвращаем свои тестовые списки
            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            //подделка-мок для User.Identity
            var controllerContext = MoqGenerator.GetControllerWithContextUserIdentity();

            //Создание самого контроллера
            DealsController dealsController = new DealsController(dealMoq.Object)
            {
                //задаем подделанный мок, для User.Identity
                ControllerContext = controllerContext.Object
            };


            //Act
            //вызов функции
            ViewResult result = dealsController.MyDeals() as ViewResult;
            var model = result.Model as List<DealModel>;
            //Assert
            dealMoq.Verify(x => x.GetAll());
            Assert.IsTrue(model.Count == dealCntPerUser);
        }

        [TestMethod]
        public void DealsEditView_ShowView()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();

            //инициализация мока помощника
            var moqHelper = MoqGenerator.GetMoqHelper();

            //списки для тестовых данных
            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            //генерация тестовых данных
            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            // отлавливаем вызов функций получения списков и возвращаем свои тестовые списки
            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            //подделка-мок для User.Identity
            var controllerContext = MoqGenerator.GetControllerWithContextUserIdentity();

            //Создание самого контроллера
            DealsController dealsController = new DealsController(dealMoq.Object)
            {
                //задаем подделанный мок, для User.Identity
                ControllerContext = controllerContext.Object
            };

            var deal = deals[0];
           
            //Act
            //вызов функции
            var result = dealsController.Edit(deal.Id) as ViewResult;
            var model = result.Model;
            //Assert
            //проверка на валидность
            Assert.AreEqual(model, deal);
        }

        [TestMethod]
        public void DealsEditPostWithFile_NotValidBoth()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var fileMoq = MoqGenerator.GetNotValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            DealsController dealsController = new DealsController(dealMoq.Object);
            var deal = new DealModel();

            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;

            //Act
            ViewResult result = dealsController.Edit(deal, fileMoq.Object) as ViewResult;
            var errorList = result.ViewData.ModelState["file"].Errors.ToList();
            //Assert
            Assert.IsTrue(errorList.Count > 0);
            Assert.IsFalse(dealsController.ModelState.IsValid);

        }

        [TestMethod]
        public void DealsEditPostWithoutFile_NotValid()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            DealsController dealsController = new DealsController(dealMoq.Object);
            var deal = new DealModel();

            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;

            //Act
            ViewResult result = dealsController.Edit(deal, null) as ViewResult;
            //Assert
            Assert.IsFalse(dealsController.ModelState.IsValid);
        }

        [TestMethod]
        public void DealsEditPostWithFile_NotValidOnlyFile()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var fileMoq = MoqGenerator.GetNotValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            var userMoq = MoqGenerator.GetControllerWithContextUserIdentity();
            DealsController dealsController = new DealsController(dealMoq.Object)
            {
                ControllerContext = userMoq.Object
            };
            var deal = deals[0];

            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;

            //Act
            var result = dealsController.Edit(deal, fileMoq.Object) as ViewResult;
            var errorList = result.ViewData.ModelState["file"].Errors.ToList();
            //Assert
            Assert.IsTrue(errorList.Count > 0);
        }

        [TestMethod]
        public void DealsEditPostWithFile_NotValidOnlyModel()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var fileMoq = MoqGenerator.GetValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            DealsController dealsController = new DealsController(dealMoq.Object);
            var deal = new DealModel();
            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;

            //Act
            ViewResult result = dealsController.Edit(deal, fileMoq.Object) as ViewResult;
            var fileError = result.ViewData.ModelState["file"];
            //Assert
            Assert.IsNull(fileError);
            Assert.IsFalse(dealsController.ModelState.IsValid);
        }

        [TestMethod]
        public void DealsEditPostWithFile_ValidBoth()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();

            var fileMoq = MoqGenerator.GetValidFile();

            //инициализация мока помощника
            var moqHelper = MoqGenerator.GetMoqHelper();

            //списки для тестовых данных
            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            //генерация тестовых данных
            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            // отлавливаем вызов функций получения списков и возвращаем свои тестовые списки
            dealMoq.Setup(x => x.GetAll()).Returns(deals);

            // мок для подмены путей сервера
            var pathProviderMoq = MoqGenerator.GetServerPathProvider();

            //подделка-мок для User.Identity
            var controllerContext = MoqGenerator.GetControllerWithContextUserIdentity();

            //Создание самого контроллера
            DealsController dealsController = new DealsController(dealMoq.Object, pp: pathProviderMoq.Object)
            {
                //задаем подделанный мок, для User.Identity
                ControllerContext = controllerContext.Object
            };

            var deal = deals[0];
            //Генерирум ошибки валидации для модели, потому что фунция 
            //в контроллере этого не сделат из-за того что функция вызвана не из представления
            dealsController = MoqGenerator.GenerateModelErrors<DealModel>(dealsController, deal) as DealsController;

            //Act
            //вызов функции
            ViewResult result = dealsController.Edit(deal, fileMoq.Object) as ViewResult;
            //Assert
            dealMoq.Verify(x => x.Update(It.IsAny<DealModel>()));
            dealMoq.Verify(x => x.Save());
            Assert.IsTrue(dealsController.ModelState.IsValid);
        }

        [TestMethod]
        public void DealsGetPhoneNumber_Correct()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var userMoq = new Mock<IRepository<AppUserModel>>();
            var fileMoq = MoqGenerator.GetValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            userMoq.Setup(x => x.GetAll()).Returns(users);


            DealsController dealsController = new DealsController(dealMoq.Object, userMoq.Object);
            var deal = deals[0];
            var user = users.Find(x => x.Id == deal.AppUserId);

            //Act
            var result = dealsController.GetPhoneNumber(deal.Id);
            var parsedObj = JsonConvert.DeserializeObject<JsonObject>(result);

            //Assert
            Assert.AreEqual(parsedObj.Status, "ok");
            Assert.AreEqual(parsedObj.Result, user.PhoneNumber);
        }

        [TestMethod]
        public void DealsDelete_NotOwn_Error()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var userMoq = new Mock<IRepository<AppUserModel>>();
            var mockingHelper = MoqGenerator.GetMoqHelper(1);
            var fileMoq = MoqGenerator.GetValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            userMoq.Setup(x => x.GetAll()).Returns(users);


            DealsController dealsController = new DealsController(dealMoq.Object, userMoq.Object, mockingHelper.Object){
                ControllerContext = MoqGenerator.GetControllerWithContextUserIdentity().Object
            };
            var deal = deals[0];
            //Act
            var result = (RedirectToRouteResult)dealsController.Delete(deal.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "MyDeals");
        }

        [TestMethod]
        public void DealsDelete_Own_Accepted()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var userMoq = new Mock<IRepository<AppUserModel>>();
            var mockingHelper = MoqGenerator.GetMoqHelper();
            var fileMoq = MoqGenerator.GetValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            userMoq.Setup(x => x.GetAll()).Returns(users);


            DealsController dealsController = new DealsController(dealMoq.Object, userMoq.Object, mockingHelper.Object)
            {
                ControllerContext = MoqGenerator.GetControllerWithContextUserIdentity().Object
            };
            var deal = deals[0];
            //Act
            var result = dealsController.Delete(deal.Id) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "Delete");
        }

        [TestMethod]
        public void DealDeleteConfirm_NotOwn_Error()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var userMoq = new Mock<IRepository<AppUserModel>>();
            var mockingHelper = MoqGenerator.GetMoqHelper(1);
            var fileMoq = MoqGenerator.GetValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            userMoq.Setup(x => x.GetAll()).Returns(users);


            DealsController dealsController = new DealsController(dealMoq.Object, userMoq.Object, mockingHelper.Object)
            {
                ControllerContext = MoqGenerator.GetControllerWithContextUserIdentity().Object
            };
            var deal = deals[0];
            //Act
            var result = (RedirectToRouteResult)dealsController.DeleteConfirmed(deal.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "MyDeals");
        }

        [TestMethod]
        public void DealsDeleteConfirmed_Own_Accepted()
        {
            //Arrange
            var dealMoq = new Mock<IRepository<DealModel>>();
            var userMoq = new Mock<IRepository<AppUserModel>>();
            var mockingHelper = MoqGenerator.GetMoqHelper();
            var fileMoq = MoqGenerator.GetValidFile();

            List<ItemModel> items = new List<ItemModel>();
            List<DealModel> deals = new List<DealModel>();
            List<AppUserModel> users = new List<AppUserModel>();

            var gen = new Generators.DataGenerator();
            gen.Generate(3, 2, 3, out users, out deals, out items);

            dealMoq.Setup(x => x.GetAll()).Returns(deals);
            userMoq.Setup(x => x.GetAll()).Returns(users);


            DealsController dealsController = new DealsController(dealMoq.Object, userMoq.Object, mockingHelper.Object)
            {
                ControllerContext = MoqGenerator.GetControllerWithContextUserIdentity().Object
            };
            var deal = deals[0];
            //Act
            var result = (RedirectToRouteResult)dealsController.DeleteConfirmed(deal.Id);


            //Assert
            dealMoq.Verify(x => x.Delete(deal.Id));
            dealMoq.Verify(x => x.Save());
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "MyDeals");
        }

    }
}
