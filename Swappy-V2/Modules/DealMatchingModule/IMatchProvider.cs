using Swappy_V2.Models;
using System.Collections.Generic;

namespace Swappy_V2.Modules.DealMatchingModule
{
    internal interface IMatchProvider
    {
        IEnumerable<AppUserModel> GetMatchedDealsOwners(DealModel deal, out List<DealModel> matchedDeals);
    }
}