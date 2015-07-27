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
        public static string TempImageFullPath { get; set; }
        public static string TempImagesPath = "/images/temp";
        public static string DealImagesPath = "/images/deals";

        public static void Init()
        {
            TempImageFullPath = HttpContext.Current.Server.MapPath(TempImagesPath);
        }

        /// <summary>
        /// 4 МБ для изображения
        /// </summary>
        public static int MaxImageLengthBytes = 4194304;
        public static string[] AllowedImageExtensions = new string[] { ".jpg", ".png", ".jpeg" };
    }
}