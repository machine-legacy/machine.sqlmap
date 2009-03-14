using System;
using System.Collections.Generic;

namespace Machine.SqlMap
{
  public static class ZipHelpers
  {
    public static IEnumerable<R> Zip<A, B, R>(this IEnumerable<A> first, IEnumerable<B> second, Func<A, B, R> zipper)
    {
      var iterator1 = first.GetEnumerator();
      var iterator2 = second.GetEnumerator();

      while (true)
      {
        bool firstHasMore = iterator1.MoveNext();
        bool secondHasMore = iterator2.MoveNext();
        if (!firstHasMore || !secondHasMore)
        {
          if (firstHasMore != secondHasMore)
          {
            throw new InvalidOperationException();
          }
          break;
        }
        yield return zipper(iterator1.Current, iterator2.Current);
      }
    }
  }
}