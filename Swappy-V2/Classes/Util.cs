using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    
    ///<summary>
    /// Helper class
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Saves file on server
        /// </summary>
        /// <param name="dir"> Save directory</param>
        /// <param name="fname">File name</param>
        /// <param name="file">File</param>
        /// <param name="pathProvider">Server path getter</param>
        /// <returns>Url to file</returns>
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