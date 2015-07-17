using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swappy_V2.Classes;
using System.Threading.Tasks;

namespace Swappy_V2.Tests
{
    [TestClass]
    public class CustomValidatorTest
    {
        [TestMethod]
        public void CityValid_Ufa_True()
        {
            //Arrange - Method is static

            //Act
            bool check = CustomValidator.CityValid("Уфа").Result;

            //Assert
            Assert.AreEqual(check, true);
        }
    }
}
