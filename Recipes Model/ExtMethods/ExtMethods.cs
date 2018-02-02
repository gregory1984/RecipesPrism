using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes_Model.ExtMethods
{
    public static class ExtMethods
    {
        public static bool IsNull(this string phrase)
        {
            return string.IsNullOrWhiteSpace(phrase);
        }

        public static bool IsNotNull(this string phrase)
        {
            return !string.IsNullOrWhiteSpace(phrase);
        }

        public static bool IsNull(this DateTime? datetime)
        {
            return datetime == null;
        }

        public static bool IsNotNull(this DateTime? datetime)
        {
            return datetime != null;
        }
    }
}
