using Moq;
using Swappy_V2.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Swappy_V2.Tests.Generators
{
    public class FakesGenerator
    {
        private string ProjectPath = @"M:\GitFiles\Swappy-V2\";
        public Controller GenerateModelErrors<ModelType>(Controller controller, ModelType model)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults);
            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
            return controller;
        }

        public Mock<IPathProvider> GetServerPathProvider()
        {
            var pathProviderMoq = new Mock<IPathProvider>();
            pathProviderMoq.Setup(x => x.MapPath(It.IsAny<string>())).Returns(
                (string path) =>
                {
                    return Path.Combine(ProjectPath + @"Swappy-V2\", path);
                });
            return pathProviderMoq;
        }

        public Mock<Mockable> GetMoqHelper(int userId = 0)
        {
            var moqHelper = new Mock<Mockable>();
            moqHelper.Setup(x => x.GetAppUserId(It.IsAny<IIdentity>())).Returns(userId);
            moqHelper.Setup(x => x.GetClaim(It.IsAny<IIdentity>(), It.IsAny<string>())).
                Returns((IIdentity id, string p) => { return p; });
            return moqHelper;
        }

        public Mock<ControllerContext> GetControllerWithContextUserIdentity(string name = "test")
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.SetupGet(x => x.Identity.Name).Returns("test");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.Setup(x => x.HttpContext.User.IsInRole(It.IsAny<string>())).
                Returns(
                (string role)
                    =>
                    {

                        return role == "User" ? true : false;
                    });
            return controllerContext;
        }

        public Mock<HttpPostedFileBase> GetValidFile()
        {
            var fileMoq = new Mock<HttpPostedFileBase>();
            fileMoq.Setup(x => x.FileName).Returns("520.jpg");
            fileMoq.Setup(x => x.ContentType).Returns("image/jpg");
            fileMoq.Setup(x => x.ContentLength).Returns(879);
            fileMoq.Setup(x => x.InputStream).Returns(
                new MemoryStream(Encoding.Default.GetBytes(File.ReadAllText(ProjectPath + @"Swappy-V2.Tests\TestResourses\520.jpg"))));
            return fileMoq;
        }

        public Mock<HttpPostedFileBase> GetNotValidFile()
        {
            var fileMoq = new Mock<HttpPostedFileBase>();
            fileMoq.Setup(x => x.FileName).Returns("sum.py");
            fileMoq.Setup(x => x.ContentType).Returns("python/py");
            fileMoq.Setup(x => x.ContentLength).Returns(879);
            fileMoq.Setup(x => x.InputStream).Returns(
                new MemoryStream(Encoding.Default.GetBytes(File.ReadAllText(ProjectPath + @"Swappy-V2.Tests\TestResourses\sum.py"))));
            return fileMoq;
        }
    }
}
