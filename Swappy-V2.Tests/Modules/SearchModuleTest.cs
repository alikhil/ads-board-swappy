using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swappy_V2.Modules;

namespace Swappy_V2.Tests.Modules
{
    [TestClass]
    public class SearchModuleTest
    {
        [TestMethod]
        public void Search_InList_Answer()
        {
            //Arrange
            var searchRequest = "Lumia 620";
            var list = new SearchableObject[]{
                new SearchableObject("Lumia 520"),
                new SearchableObject("Lumia 620"),
                new SearchableObject("Lumia 6200"),
                new SearchableObject("Lumia 6520"),
                new SearchableObject("Nexus 20"),
                new SearchableObject("Iphone 5")
            };
            //Act
            var result = SearchModule.FindOut(searchRequest, list);
            var answ = result.FullMatch.Count == 1 && result.FullMatch[0].Key == list[1] &&
                        result.FullSubstringMatch.Count == 1 && result.FullSubstringMatch[0].Key == list[2] &&
                        result.IncompleteMatch.Count == 2 && result.IncompleteMatch[0].Key == list[0] && result.IncompleteMatch[1].Key == list[3];
           
            //Assert
            Assert.AreEqual(answ, true);
        }
    }

    public class SearchableObject : Searchable
    {
        public string Value { get; set; }
        public SearchableObject() { }
        public SearchableObject(string val)
        {
            Value = val;
        }
        public string SearchBy()
        {
            return Value;
        }
    }
}
