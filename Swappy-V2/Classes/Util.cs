using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    public static class Util
    {
        public static string SaveFile(string dir, string fname, HttpPostedFileBase file, IPathProvider pathProvider = null)
        {
            var serverPathProvider = pathProvider == null ? new ServerPathProvider() : pathProvider;
            string ans = Path.Combine(Path.Combine(dir, fname));
            dir = "~" + dir;
            
            string path = Path.Combine(serverPathProvider.MapPath(dir), fname);
            file.SaveAs(path);
            return ans;
        }
    }
}