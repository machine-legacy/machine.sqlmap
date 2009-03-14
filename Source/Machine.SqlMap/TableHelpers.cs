using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.SqlMap
{
  public static class TableHelpers
  {
    public static IProjectedTable ToTable<T>(this IEnumerable<T> enumerable, string name, Func<T, object> action)
    {
      Table table = new Table(new Column(name, 0, typeof(T)));
      return new StaticTable(table, enumerable.Select(x => new object[] { x }).ToArray());
    }
  }
}