using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace Swappy_V2.Classes.Extensions
{
    public static class IdentityUserExtensions
    {
        static Logger Logger = LogManager.GetCurrentClassLogger();
        public static int GetAppUserId(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaim(identity,"AppUserId"));
        }
        public static string GetClaim(this IIdentity identity, string claim)
        {
            string claimRes = null;
            try
            {
                var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
                claimRes = prinicpal.Claims.Where(c => c.Type == claim).Select(c => c.Value).SingleOrDefault();
            }
            catch (Exception e)
            {
                var messages = e.GetAllMessages();
                var ident = identity.ToString();
                Logger.Error(String.Format("Ошибка в Identity.Get{2} : {0}\nidentity : {1}", messages, ident, claim));
            }
            return claimRes;
        }
        public static string ToString(this IIdentity identity)
        {
            return JsonConvert.SerializeObject(identity);
        }
    }
}