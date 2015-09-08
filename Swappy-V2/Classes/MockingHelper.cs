using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web;
namespace Swappy_V2.Classes
{
    /// <summary>
    /// Класс для помощи мокания статик и прочих функций, которые трудно отмокать
    /// </summary>
    public class MockingHelper : Mockable
    {
        public int GetAppUserId(IIdentity identity)
        {
            return identity.GetAppUserId();
        }

        public string GetClaim(IIdentity identity, string p)
        {
            return identity.GetClaim(p);
        }
    }
    public interface Mockable
    {
        int GetAppUserId(IIdentity identity);
        string GetClaim(IIdentity identity, string p);
    }
    
    public interface IPathProvider
    {
        string MapPath(string path);
    }

    public class ServerPathProvider : IPathProvider
    {
        public string MapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}
