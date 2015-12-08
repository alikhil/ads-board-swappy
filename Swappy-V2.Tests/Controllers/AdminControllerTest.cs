using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swappy_V2.Models;
using Moq;
using Swappy_V2.Tests.Generators;
using Swappy_V2.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Swappy_V2.Tests.Controllers
{
    [TestClass]
    public class AdminControllerTest
    {
        FakesGenerator MoqGenerator;
        public AdminControllerTest()
        {
            MoqGenerator = new FakesGenerator();
        }

        [TestMethod]
        public void Admin_Deals_ReturnAllDeals()
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

            AdminController adminController = new AdminController(dealMoq.Object, userMoq.Object, mockingHelper.Object)
            {
                ControllerContext = MoqGenerator.GetControllerWithContextUserIdentity().Object
            };

            var deal = deals[0];
            //Act
            var result = adminController.Deals() as ViewResult;
            var model = (List<DealModel>)result.Model;
            //Assert
            dealMoq.Verify(x => x.GetAll());
            CollectionAssert.AreEqual(deals, model);
        }

        [TestMethod]
        public void Admin_Users_ReturnAllUsers()
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

            AdminController adminController = new AdminController(dealMoq.Object, userMoq.Object, mockingHelper.Object)
            {
                ControllerContext = MoqGenerator.GetControllerWithContextUserIdentity().Object
            };

            var deal = deals[0];
            //Act
            var result = adminController.Users() as ViewResult;
            var model = (List<AppUserModel>)result.Model;
            //Assert
            userMoq.Verify(x => x.GetAll());
            CollectionAssert.AreEqual(users, model);
        }

        [TestMethod]
        public void Admin_ShowUserValidUserId_Accept()
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

            AdminController adminController = new AdminController(dealMoq.Object, userMoq.Object, mockingHelper.Object)
            {
                ControllerContext = MoqGenerator.GetControllerWithContextUserIdentity().Object
            };
            var usersDeal = deals.FindAll(x => x.AppUserId == users[0].Id);
            //Act
            var result = adminController.ShowUser(users[0].Id) as ViewResult;
            var model = result.Model;
            List<DealModel> dealsFromResult = result.ViewBag.Deals;
            //Assert
            userMoq.Verify(x => x.GetAll());
            Assert.AreEqual(users[0], model);
            Assert.IsTrue(dealsFromResult.TrueForAll(x => usersDeal.Contains(x)));
            
        }

        [TestMethod]
        public void Admin_ShowUserThatNotExist_Error()
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

            AdminController adminController = new AdminController(dealMoq.Object, userMoq.Object, mockingHelper.Object)
            {
                ControllerContext = MoqGenerator.GetControllerWithContextUserIdentity().Object
            };
            //Act
            var result = adminController.ShowUser(4) as RedirectToRouteResult;
            //Assert
            userMoq.Verify(x => x.GetAll());
            Assert.AreEqual(result.RouteValues["action"], "Index");
        }
    }
}
