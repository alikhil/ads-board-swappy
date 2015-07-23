using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swappy_V2.Models;

namespace Swappy_V2.Classes
{
    public static class DealModelExtensions
    {
        public static string SearchBy(this DealModel model)
        {
            return model.ItemToChange.Title;
        }
    }
}