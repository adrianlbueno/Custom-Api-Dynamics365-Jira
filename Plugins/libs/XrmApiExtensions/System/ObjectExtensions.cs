using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ObjectExtensions
    {
        public static String ToString<T>(this T obj, Func<T, String> func)
        {
            return obj != null ? func?.Invoke(obj) : String.Empty;
        }

        public static T Set<T>(this T obj, params Action<T>[] actions)
        {
            actions?.Foreach(action => action.Invoke(obj));
            return obj;
        }
    }
}
