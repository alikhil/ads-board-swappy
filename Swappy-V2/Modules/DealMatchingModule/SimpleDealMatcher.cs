using Swappy_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Modules.DealMatchingModule
{
    public class SimpleDealMatcher : IDealMatcher
    {
        public bool IsMatch(DealModel a, DealModel b)
        {
            return a.Variants.Any(d => DealMatchingModule.Instance.Matches(d.Title, b.Title))
                && b.Variants.Any(d => DealMatchingModule.Instance.Matches(d.Title, a.Title));
        }
    }
}