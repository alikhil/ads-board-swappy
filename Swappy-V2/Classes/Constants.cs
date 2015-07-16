using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    public static class AppConstants
    {
        public static string KladrId = "***REMOVED***";
        public static string SendgridAccount = "alikhil";
        public static string SendgridPassword = "***REMOVED***";

        public static string NoReplyMailAddress = "noreply@swappy.ru";
        public static string SupportMailAddress = "support@swappy.ru";
        public static string AdminMailAddress = "admin@swappy.ru";

        public static string NoImagePath = "/Images\\noImage.gif";
        public static string TempImageFullPath = HttpContext.Current.Server.MapPath(TempImagesPath);
        public static string TempImagesPath = "/images/temp";
        public static string DealImagesPath = "/images/deals";
    }
}