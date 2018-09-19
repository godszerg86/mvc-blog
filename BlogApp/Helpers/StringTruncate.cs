using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogApp.Helpers
{
    public class StringTruncate
    {
        public static string Truncate(string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
            }
            return source;
        }

    }
}