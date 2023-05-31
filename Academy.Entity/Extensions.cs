using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Entity;


public sealed class Extensions
{
    public static string EnumToString<T>(T value) where T : struct
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException();
        }

        var fieldName = Enum.GetName(typeof(T), value);
        return fieldName ?? string.Empty;
    }
}

