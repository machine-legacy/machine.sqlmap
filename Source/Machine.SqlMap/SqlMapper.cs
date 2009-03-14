using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Machine.SqlMap
{
  public class SqlMapper
  {
    public IEnumerable<T> Map<T>(Table table, object[][] rows)
    {
      return Map<T>(new StaticTable(table, rows));
    }

    public IEnumerable<T> Map<T>(IDataReader reader)
    {
      return Map<T>(new ReaderTable(reader));
    }

    public IEnumerable<T> Map<T>(IProjectedTable projectedTable)
    {
      return Map(typeof(T), projectedTable).Cast<T>();
    }

    public IEnumerable<object> Map(Type mappedType, IProjectedTable projectedTable)
    {
      TypeAttributes attributes = TypeAttributes.For(mappedType);
      Factory factory = attributes.ToFactory(projectedTable);
      return projectedTable.Rows().Select(x => factory.Create(x));
    }
  }

  public static class MoreEnumerables
  {
    public static IEnumerable<T> UniqueOn<T>(this IEnumerable<T> enumerable, Func<T, object> on)
    {
      foreach (var byKey in enumerable.ToLookup(on))
      {
        yield return byKey.First();
      }
    }
  }
}