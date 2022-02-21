using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedditAnswerGenerator.Tests
{
    public static class EnumerableExtensions
    {
        public static int GetSequenceHashCode<TItem>(this IEnumerable<TItem> list)
        {
            if (list == null) return 0;
            const int seedValue = 0x2D2816FE;
            const int primeNumber = 397;
            return list.Aggregate(seedValue, (current, item) => (current * primeNumber) + (Equals(item, default(TItem)) ? 0 : item.GetHashCode()));
        }
    }

    class EdgeTupleEqualityComparer : IEqualityComparer<Tuple<List<string>, bool>>
    {
        public bool Equals(Tuple<List<string>, bool> b1, Tuple<List<string>, bool> b2)
        {
            if (b1.Item1.SequenceEqual(b2.Item1) && b1.Item2 == b2.Item2)
                return true;

            return false;
        }

        public int GetHashCode(Tuple<List<string>, bool> bx)
        {
            return EnumerableExtensions.GetSequenceHashCode(bx.Item1) ^ bx.Item2.GetHashCode();
        }
    }

    public class GraphTupleEqualityComparer : IEqualityComparer<Tuple<List<string>, bool, List<string>>>
    {
        public bool Equals(Tuple<List<string>, bool, List<string>> b1, Tuple<List<string>, bool, List<string>> b2)
        {
            if (b1.Item1.SequenceEqual(b2.Item1) && b1.Item2 == b2.Item2 && b1.Item3.SequenceEqual(b2.Item3))
                return true;

            return false;
        }

        public int GetHashCode(Tuple<List<string>, bool, List<string>> bx)
        {
            return EnumerableExtensions.GetSequenceHashCode(bx.Item1) ^ bx.Item2.GetHashCode() ^ EnumerableExtensions.GetSequenceHashCode(bx.Item3);
        }
    }
}
