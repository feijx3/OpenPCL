using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MeloongCore.Extensions
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class EnumerableExtensions
    {
        [return: Nullable(2)]
        public static TSource FirstOrDefault<[Nullable(2)] TSource>(this IEnumerable<TSource> source, params Func<TSource, bool>[] predicates)
        {
            foreach (Func<TSource, bool> func in predicates)
            {
                foreach (TSource tsource in source)
                {
                    if (func(tsource))
                    {
                        return tsource;
                    }
                }
            }
            return default(TSource);
        }

        public static IEnumerable<T> Distinct<[Nullable(2)] T>(this IList<T> list, Func<T, T, bool> comparer)
        {
            EnumerableExtensions.<Distinct>d__1<T> <Distinct>d__ = new EnumerableExtensions.<Distinct>d__1<T>(-2);
            <Distinct>d__.<>3__list = list;
            <Distinct>d__.<>3__comparer = comparer;
            return <Distinct>d__;
        }

        public static IEnumerable<TSource> DistinctBy<[Nullable(2)] TSource, [Nullable(2)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            EnumerableExtensions.<DistinctBy>d__2<TSource, TKey> <DistinctBy>d__ = new EnumerableExtensions.<DistinctBy>d__2<TSource, TKey>(-2);
            <DistinctBy>d__.<>3__source = source;
            <DistinctBy>d__.<>3__selector = selector;
            return <DistinctBy>d__;
        }

        public static ParallelQuery<TSource> DistinctBy<[Nullable(2)] TSource, [Nullable(2)] TKey>(this ParallelQuery<TSource> source, Func<TSource, TKey> selector)
        {
            ConcurrentDictionary<TKey, bool> seen = new ConcurrentDictionary<TKey, bool>();
            return source.Where<TSource>((TSource element) => seen.TryAdd(selector(element), true));
        }

        public static List<TSource> MaxByAll<[Nullable(2)] TSource, [Nullable(0)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
        {
            List<TSource> list = new List<TSource>();
            List<TSource> list2;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    list2 = list;
                }
                else
                {
                    TSource tsource = enumerator.Current;
                    TKey tkey = selector(tsource);
                    list.Add(tsource);
                    while (enumerator.MoveNext())
                    {
                        TSource tsource2 = enumerator.Current;
                        TKey tkey2 = selector(tsource2);
                        int num = tkey2.CompareTo(tkey);
                        if (num > 0)
                        {
                            tkey = tkey2;
                            list.Clear();
                            list.Add(tsource2);
                        }
                        else if (num == 0)
                        {
                            list.Add(tsource2);
                        }
                    }
                    list2 = list;
                }
            }
            return list2;
        }

        public static List<TSource> MaxByAll<[Nullable(2)] TSource, [Nullable(2)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            List<TSource> list = new List<TSource>();
            List<TSource> list2;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    list2 = list;
                }
                else
                {
                    TSource tsource = enumerator.Current;
                    TKey tkey = selector(tsource);
                    list.Add(tsource);
                    while (enumerator.MoveNext())
                    {
                        TSource tsource2 = enumerator.Current;
                        TKey tkey2 = selector(tsource2);
                        int num = comparer.Compare(tkey2, tkey);
                        if (num > 0)
                        {
                            tkey = tkey2;
                            list.Clear();
                            list.Add(tsource2);
                        }
                        else if (num == 0)
                        {
                            list.Add(tsource2);
                        }
                    }
                    list2 = list;
                }
            }
            return list2;
        }

        public static List<TSource> MinByAll<[Nullable(2)] TSource, [Nullable(0)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
        {
            List<TSource> list = new List<TSource>();
            List<TSource> list2;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    list2 = list;
                }
                else
                {
                    TSource tsource = enumerator.Current;
                    TKey tkey = selector(tsource);
                    list.Add(tsource);
                    while (enumerator.MoveNext())
                    {
                        TSource tsource2 = enumerator.Current;
                        TKey tkey2 = selector(tsource2);
                        int num = tkey2.CompareTo(tkey);
                        if (num < 0)
                        {
                            tkey = tkey2;
                            list.Clear();
                            list.Add(tsource2);
                        }
                        else if (num == 0)
                        {
                            list.Add(tsource2);
                        }
                    }
                    list2 = list;
                }
            }
            return list2;
        }

        public static List<TSource> MinByAll<[Nullable(2)] TSource, [Nullable(2)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            List<TSource> list = new List<TSource>();
            List<TSource> list2;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    list2 = list;
                }
                else
                {
                    TSource tsource = enumerator.Current;
                    TKey tkey = selector(tsource);
                    list.Add(tsource);
                    while (enumerator.MoveNext())
                    {
                        TSource tsource2 = enumerator.Current;
                        TKey tkey2 = selector(tsource2);
                        int num = comparer.Compare(tkey2, tkey);
                        if (num < 0)
                        {
                            tkey = tkey2;
                            list.Clear();
                            list.Add(tsource2);
                        }
                        else if (num == 0)
                        {
                            list.Add(tsource2);
                        }
                    }
                    list2 = list;
                }
            }
            return list2;
        }

        [return: Nullable(2)]
        public static TSource MaxBy<[Nullable(2)] TSource, [Nullable(0)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
        {
            TSource tsource;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    tsource = default(TSource);
                    tsource = tsource;
                }
                else
                {
                    TSource tsource2 = enumerator.Current;
                    TKey tkey = selector(tsource2);
                    while (enumerator.MoveNext())
                    {
                        TSource tsource3 = enumerator.Current;
                        TKey tkey2 = selector(tsource3);
                        if (tkey2.CompareTo(tkey) > 0)
                        {
                            tsource2 = enumerator.Current;
                            tkey = tkey2;
                        }
                    }
                    tsource = tsource2;
                }
            }
            return tsource;
        }

        [return: Nullable(2)]
        public static TSource MaxBy<[Nullable(2)] TSource, [Nullable(2)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            TSource tsource;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    tsource = default(TSource);
                    tsource = tsource;
                }
                else
                {
                    TSource tsource2 = enumerator.Current;
                    TKey tkey = selector(tsource2);
                    while (enumerator.MoveNext())
                    {
                        TSource tsource3 = enumerator.Current;
                        TKey tkey2 = selector(tsource3);
                        if (comparer.Compare(tkey2, tkey) > 0)
                        {
                            tsource2 = enumerator.Current;
                            tkey = tkey2;
                        }
                    }
                    tsource = tsource2;
                }
            }
            return tsource;
        }

        [return: Nullable(2)]
        public static TSource MinBy<[Nullable(2)] TSource, [Nullable(0)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) where TKey : IComparable<TKey>
        {
            TSource tsource;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    tsource = default(TSource);
                    tsource = tsource;
                }
                else
                {
                    TSource tsource2 = enumerator.Current;
                    TKey tkey = selector(tsource2);
                    while (enumerator.MoveNext())
                    {
                        TSource tsource3 = enumerator.Current;
                        TKey tkey2 = selector(tsource3);
                        if (tkey2.CompareTo(tkey) < 0)
                        {
                            tsource2 = enumerator.Current;
                            tkey = tkey2;
                        }
                    }
                    tsource = tsource2;
                }
            }
            return tsource;
        }

        [return: Nullable(2)]
        public static TSource MinBy<[Nullable(2)] TSource, [Nullable(2)] TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            TSource tsource;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    tsource = default(TSource);
                    tsource = tsource;
                }
                else
                {
                    TSource tsource2 = enumerator.Current;
                    TKey tkey = selector(tsource2);
                    while (enumerator.MoveNext())
                    {
                        TSource tsource3 = enumerator.Current;
                        TKey tkey2 = selector(tsource3);
                        if (comparer.Compare(tkey2, tkey) < 0)
                        {
                            tsource2 = enumerator.Current;
                            tkey = tkey2;
                        }
                    }
                    tsource = tsource2;
                }
            }
            return tsource;
        }

        public static IEnumerable<T> Ordered<[Nullable(0)] T>(this IEnumerable<T> source) where T : IComparable<T>
        {
            return source.OrderBy<T, T>((T x) => x);
        }

        public static IEnumerable<T> OrderedDescending<[Nullable(0)] T>(this IEnumerable<T> source) where T : IComparable<T>
        {
            return source.OrderByDescending<T, T>((T x) => x);
        }

        public static string Join(this IEnumerable list, char split)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            foreach (object obj in list)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    stringBuilder.Append(split);
                }
                if (obj != null)
                {
                    stringBuilder.Append(obj.ToString());
                }
            }
            return stringBuilder.ToString();
        }

        public static string Join(this IEnumerable list, string split)
        {
            if (split.IsSingle<char>())
            {
                return list.Join(split[0]);
            }
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            foreach (object obj in list)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    stringBuilder.Append(split);
                }
                if (obj != null)
                {
                    stringBuilder.Append(obj);
                }
            }
            return stringBuilder.ToString();
        }

        public static bool Remove<[Nullable(2)] TKey, [Nullable(2)] TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            TValue tvalue;
            return dict.TryRemove(key, out tvalue);
        }

        public static Dictionary<T, T> ToDictionary<T>(this T[,] arr)
        {
            Dictionary<T, T> dictionary = new Dictionary<T, T>();
            if (arr.Length == 0)
            {
                return dictionary;
            }
            if (arr.GetLength(1) != 2)
            {
                throw new ArgumentException("数组必须为两列，第一列为 Key，第二列为 Value。");
            }
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                dictionary[arr[i, 0]] = arr[i, 1];
            }
            return dictionary;
        }

        [NullableContext(2)]
        public static TValue GetOrDefault<TKey, TValue>([Nullable(1)] this IDictionary<TKey, TValue> dict, [Nullable(1)] TKey key, TValue defaultValue = default(TValue))
        {
            TValue tvalue;
            if (!dict.TryGetValue(key, out tvalue))
            {
                return defaultValue;
            }
            return tvalue;
        }

        public static void AddIntoValueCollection<[Nullable(2)] TKey, [Nullable(2)] TValue>(this IDictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        {
            List<TValue> list;
            if (dict.TryGetValue(key, out list))
            {
                list.Add(value);
                return;
            }
            dict.Add(key, new List<TValue>(1) { value });
        }

        [NullableContext(0)]
        [return: Nullable(new byte[] { 1, 0 })]
        public static IEnumerable<T> Flags<T>(this T value) where T : struct, Enum
        {
            return from element in EnumUtils.GetAllFlags<T>()
                where value.HasFlag(element)
                select element;
        }

        public static void RemoveIf<[Nullable(2)] T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            foreach (T t in source.Where<T>(predicate).ToList<T>())
            {
                source.Remove(t);
            }
        }

        public static void RemoveAll<[Nullable(2)] T>(this ICollection<T> source, IEnumerable<T> itemsToRemove)
        {
            foreach (T t in itemsToRemove)
            {
                source.Remove(t);
            }
        }

        public static void KeepIf<[Nullable(2)] T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            foreach (T t in source.Where<T>((T item) => !predicate(item)).ToList<T>())
            {
                source.Remove(t);
            }
        }

        public static IEnumerable<T> Except<[Nullable(2)] T>(this IEnumerable<T> source, T element)
        {
            return source.Except<T>(new <>z__ReadOnlySingleElementList<T>(element));
        }

        [NullableContext(2)]
        public static bool IsSingle<T>([Nullable(new byte[] { 2, 1 })] this IEnumerable<T> source)
        {
            if (source == null)
            {
                return false;
            }
            IList<T> list = source as IList<T>;
            if (list != null)
            {
                return list.Count == 1;
            }
            bool flag;
            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    flag = false;
                }
                else if (enumerator.MoveNext())
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            return flag;
        }

        public static IEnumerable<T> ForAll<[Nullable(2)] T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T t in source)
            {
                action(t);
            }
            return source;
        }
    }
}
