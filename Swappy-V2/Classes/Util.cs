using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    public static class Util
    {
        public static string SaveFile(string dir, string fname, HttpPostedFileWrapper file)
        {

            string ans = Path.Combine(Path.Combine(dir, fname));
            dir = "~" + dir;
            string path = Path.Combine(HttpContext.Current.Server.MapPath(dir), fname);
            file.SaveAs(path);
            return ans;
        }
    }
}