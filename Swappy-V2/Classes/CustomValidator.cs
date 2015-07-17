using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Swappy_V2.Modules.KladrModule;
namespace Swappy_V2.Classes
{
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
    }
}