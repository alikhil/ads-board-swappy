using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Classes
{
    public static class ExceptionExtensions
    {
        public static string GetAllMessages(this Exception e)
        {
            var messages = "";
            var cur = e;
            while (cur != null)
            {
                messages += cur.Message + '\n';
                cur = cur.InnerException;
            }
            return messages;
        }
    }
}