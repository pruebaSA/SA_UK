namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class EnumerableExtensions
    {
        public static void ForEach<TItem>(this IEnumerable<TItem> sequence, Action<TItem> action)
        {
            foreach (TItem local in sequence)
            {
                action(local);
            }
        }

        public static string JoinStrings<TItem>(this IEnumerable<TItem> sequence, string separator) => 
            sequence.JoinStrings<TItem>(separator, item => item.ToString());

        public static string JoinStrings<TItem>(this IEnumerable<TItem> sequence, string separator, Func<TItem, string> converter)
        {
            StringBuilder seed = new StringBuilder();
            sequence.Aggregate<TItem, StringBuilder>(seed, delegate (StringBuilder builder, TItem item) {
                if (builder.Length > 0)
                {
                    builder.Append(separator);
                }
                builder.Append(converter(item));
                return builder;
            });
            return seed.ToString();
        }
    }
}

