using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI.WebControls;

namespace Swappy_V2.Classes
{
    public static class ValidationUIHelpers
    {
        public static MvcHtmlString FilteredValidationSummary(this HtmlHelper html, string filterPrefix)
        {
            var matchingErrors = from e in html.ViewData.ModelState
                                 where e.Key.StartsWith(filterPrefix)
                                 from x in e.Value.Errors
                                 select new { key = e.Key, msg = x.ErrorMessage };
            var vd = new ViewDataDictionary();
            foreach (var matchingError in matchingErrors)
                vd.ModelState.AddModelError(matchingError.key, matchingError.msg);
            var html2 = new HtmlHelper(html.ViewContext, new FakeViewDataContainer { ViewData = vd });

            return html2.ValidationSummary("", new { @class = "text-danger" });
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}