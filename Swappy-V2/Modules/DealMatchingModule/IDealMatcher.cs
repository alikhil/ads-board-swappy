using Swappy_V2.Models;
using Swappy_V2.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Modules.DealMatchingModule

{
    public interface IDealMatcher
    {
        bool IsMatch(DealModel a, DealModel b);
    }
}