using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class EnumExtensions
    {
        public static TEnum? TryParseToEnum<TEnum>(this String input) where TEnum : struct, Enum
        {
            TEnum retVal;
            return (Enum.TryParse(input, out retVal) ? retVal : (null as TEnum?));
        }

        public static TEnum? TryParseToEnum<TEnum>(this int input) where TEnum : struct, Enum
        {
            return (Enum.IsDefined(typeof(TEnum), input) ? (TEnum)Enum.ToObject(typeof(TEnum), input) : (null as TEnum?));
        }
    }
}
