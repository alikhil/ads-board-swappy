using System;
using System.Collections.Generic;
using Swappy_V2.Models;
using System.Linq;

namespace Swappy_V2.Modules.DealMatchingModule
{
    internal class SimpleMatchProvider : IMatchProvider
    {
        public IEnumerable<AppUserModel> GetMatchedDealsOwners(DealModel deal, out List<DealModel> matchedDeals)
        {
            IDealMatcher matcher = new SimpleDealMatcher();
            var dealRepositroy = new DealsRepository();
            var userRepository = new UsersRepository();
            matchedDeals = dealRepositroy
                .GetAll()
                .Where(d => matcher.IsMatch(deal, d))
                .ToList();

            var appUsers = matchedDeals
                .Select(d => d.AppUserId)
                .Where(appUserId => appUserId != deal.AppUserId)
                .Distinct()
                .Select(id => userRepository
                    .GetAll()
                    .FirstOrDefault(u => u.Id == id));
            return appUsers;
        }
    }
}