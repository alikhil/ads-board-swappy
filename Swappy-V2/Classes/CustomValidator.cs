using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Swappy_V2.Modules.KladrModule;
using System.Globalization;
using System.IO;
namespace Swappy_V2.Classes
{
    /// <summary>
    /// Класс для проверки валидности 
    /// </summary>
    public static class CustomValidator
    {
        public static async Task<bool> CityValid(string city)
        {
            KladrClient client = new KladrClient("", AppConstants.KladrId);
            var resp = await client.Check(new Dictionary<string, string>
                                        {
                                            {"query", city },
                                            {"contentType", "city"},
                                            {"withParent", "1"}
                                        });
            return resp == city;
        }

        public static ValidStatus ImageVaild(HttpPostedFileBase file)
        {
            List<string> errors = new List<string>();
            ValidStatus result = ValidStatus.Unknown;
            if (file != null && file.ContentLength > 0)
            {
                if (file.ContentLength > AppConstants.MaxImageLengthBytes)
                    result |= ValidStatus.MaxLengthOverload | ValidStatus.NotValid;

                if (!file.ContentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/"))
                    result |= ValidStatus.IncorrectType | ValidStatus.NotValid;

                var ext = Path.GetExtension(file.FileName);
                if (!AppConstants.AllowedImageExtensions.Contains(ext.ToLower()))
                    result |= ValidStatus.IncorrectFormat| ValidStatus.NotValid;

            }
            else
                result = ValidStatus.Empty;
            result = (result & ValidStatus.NotValid) == ValidStatus.NotValid ? result : ValidStatus.Valid;
            result ^= ValidStatus.Unknown;
            return result;
        }
    }
    public enum ValidStatus
    {
        Unknown = 0x0,
        Valid = 0x01,
        MaxLengthOverload = 0x02,
        IncorrectFormat = 0x04,
        IncorrectType = 0x08,
        NotValid = 0x10,
        Empty = 0x20
    }
}
