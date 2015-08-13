using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Swappy_V2.Classes
{
    public class AdminFilter : AuthorizeAttribute
    {
        private string[] allowedRoles = new string[]{ "Admin", "Moderator"};
        private bool roleResult = false;
        public AdminFilter()
        {
        }
 
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            roleResult = Role(httpContext);
            return httpContext.Request.IsAuthenticated && roleResult;
                
        }
 
        private bool Role(HttpContextBase httpContext)
        {
            if (allowedRoles.Length > 0)
            {
                for (int i = 0; i < allowedRoles.Length; i++)
                {
                    if (httpContext.User.IsInRole(allowedRoles[i]))
                        return true;
                }
                return false;
            }
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if(roleResult)
                    base.HandleUnauthorizedRequest(filterContext);
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Deals", action = "Index" }));
                }
            }
            else
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Account", action = "Login", returnUrl = "/Admin" }
                    ));
            
        }
    }
}