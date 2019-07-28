using System;
using System.Collections.Generic;
using System.Linq;
using CustomTypes;

namespace Utils
{
    public static class Helpers
    {
        public static bool HasLength(this string val) => !string.IsNullOrWhiteSpace(val);

        public static bool HasLength<T>(this IEnumerable<T> val) => val != null && val.Any();

        public static bool IsDefault<T>(this T val) => EqualityComparer<T>.Default.Equals(val, default(T));

        public static bool IsNull(this object val) => val == null;

        public static bool IsTrue(this bool val) => val == true;

        public static bool IsFalse(this bool val) => val == false;

        public static void WriteLine(this string val)
        {
            if (val.HasLength())
                Console.WriteLine(val);
        }

        public static Monad<int> ToInt(this string val)
        {
            int intVal = 0;
            bool isValid = int.TryParse(val, out intVal);
            if(isValid)
                return intVal.ToMonad();
            else
                return Monad.None<int>();
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            using (var e = source.GetEnumerator())
                while (e.MoveNext()) action.Invoke(e.Current);
            return source;
        }

        public static IDictionary<T, T2> ForEach<T, T2>(this IDictionary<T, T2> source, Action<KeyValuePair<T, T2>> action)
        {
            using (var e = source.GetEnumerator())
                while (e.MoveNext()) action.Invoke(e.Current);
            return source;
        }

        public static void CopyTo<T>(this IEnumerable<T> source, ICollection<T> target)
        {
            target.Clear();
            source.ForEach(item => target.Add(item));
        }


    }
}
