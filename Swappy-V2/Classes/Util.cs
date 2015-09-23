using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    /// <summary>
    /// Вспомогательный класс
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Сохранение файла на сервере
        /// </summary>
        /// <param name="dir"> Дириктория где сохранять файлы</param>
        /// <param name="fname">Имя файла</param>
        /// <param name="file">Сам файл</param>
        /// <param name="pathProvider">Объект для получения пути на сервере</param>
        /// <returns></returns>
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