using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    public static class EnumUtils
    {
        [return: Nullable(new byte[] { 1, 0 })]
        private static IEnumerable<T> GetAllValues<T>() where T : struct, Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T FromString<T>([Nullable(2)] string value) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value))
            {
                return (T)((object)Enum.ToObject(typeof(T), 0));
            }
            double num;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out num))
            {
                return (T)((object)Enum.ToObject(typeof(T), Convert.ToInt64(num)));
            }
            return (T)((object)Enum.Parse(typeof(T), value, true));
        }

        public static bool HasFlagF<T>(this T value, T flag) where T : struct, Enum
        {
            return value.HasFlag(flag) && !EqualityComparer<T>.Default.Equals(flag, default(T));
        }

        [return: Nullable(new byte[] { 1, 0 })]
        public static List<T> GetAllFlags<T>() where T : struct, Enum
        {
            List<Enum> list;
            if (EnumUtils.allFlagsCache.TryGetValue(typeof(T), out list))
            {
                return list.Cast<T>().ToList<T>();
            }
            if (!typeof(T).IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException("枚举 " + typeof(T).FullName + " 没有定义 FlagsAttribute。");
            }
            List<T> list2 = new List<T>();
            foreach (T t in EnumUtils.GetAllValues<T>().Distinct<T>())
            {
                long num = Convert.ToInt64(t, CultureInfo.InvariantCulture);
                if (num != 0L && (num & (num - 1L)) == 0L)
                {
                    list2.Add(t);
                }
            }
            EnumUtils.allFlagsCache[typeof(T)] = list2.Cast<Enum>().ToList<Enum>();
            return list2;
        }

        [Nullable(1)]
        private static readonly ConcurrentDictionary<Type, List<Enum>> allFlagsCache = new ConcurrentDictionary<Type, List<Enum>>();
    }
}
