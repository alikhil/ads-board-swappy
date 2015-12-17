using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using Swappy_V2.Models;
using Swappy_V2.Classes.Extensions;

namespace Swappy_V2.Classes
{
    /// <summary>
    /// Helper class for mocking 
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

        public string GetUserId(IIdentity identity)
        {
            return identity.GetUserId();
        }

        public object GetSameObject(object obj)
        {
            return obj;
        }
    }

    /// <summary>
    /// Interface of Helper class for mocking static functions
    /// </summary>
    public interface Mockable
    {
        int GetAppUserId(IIdentity identity);
        string GetClaim(IIdentity identity, string p);
        string GetUserId(IIdentity identity);
        object GetSameObject(object obj);
    }
    
    /// <summary>
    /// Interface for getting server path
    /// Вынесен в отдельный интерфейс для удобства тестирования
    /// </summary>
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
