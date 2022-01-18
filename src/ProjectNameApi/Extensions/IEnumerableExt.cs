using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ProjectNameApi.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExt
    {
        public static string JoinWithQuotes<T>(this IEnumerable<T> src, string separator = ",",
            bool doubleQuotes = false)
        {
            var quote = doubleQuotes ? "\"" : "'";

            return string.Join(separator, src.Select(a => $"{quote}{a}{quote}"));
        }

        public static string JoinWithPrefixOrSuffix<T>(this IEnumerable<T> src, string prefix = "", string suffix = "",
            string separator = "")
        {
            return string.Join(separator, src.Select(a => $"{prefix}{a}{suffix}"));
        }

        public static string JoinToString(this IEnumerable<string> src, string separator = ", ", bool removeEmpty = true)
        {
            if (removeEmpty)
                return string.Join(separator, src.Where(a => !string.IsNullOrEmpty(a)));
            
            return string.Join(separator, src);
        }

        public static string JoinToStringSanitized(this IEnumerable<string> src, bool withLike = false,
            string separator = ", ")
        {
            return src.Select(a => a.SanitizeSql(withLike)).JoinToString(separator);
        }

        public static string JoinToString<T>(this IEnumerable<T> src, string separator = ", ")
        {
            return string.Join(separator, src.Select(a => a.ToString()));
        }

        public static bool None<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool None<T>(this IEnumerable<T> source,
            Func<T, bool> predicate)
        {
            return source == null || !source.Any(predicate);
        }

        public static void AddIfUnique<T>(this List<T> source, List<T> items)
        {
            if (source == null || items == null)
                return;

            foreach (var item in items.Where(item => !source.Contains(item)))
                source.Add(item);
        }

        public static void AddIfUnique<T>(this List<T> source, T item)
        {
            if (!source.Contains(item))
                source.Add(item);
        }

        public static List<string> ToListSerialized<T>(this List<T> source)
        {
            return source.Select(a => JsonSerializer.Serialize(a)).ToList();
        }

        public static List<T> ToListDeserialized<T>(this List<string> source)
        {
            return source.Select(a => JsonSerializer.Deserialize<T>(a)).ToList();
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

        /// <summary>
        /// Selects an item that has the max value of specified property
        /// </summary>
        public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R> {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;
        }

        /// <summary>
        /// Selects an item that has the min value of specified property
        /// </summary>
        public static T MinBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R> {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) < 0 ? next : max).Item1;
        }
    }
}