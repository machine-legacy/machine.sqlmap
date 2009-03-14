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
      Table table = projectedTable.ToTable();
      Factory factory = attributes.ToFactory(projectedTable);
      var rows = projectedTable.Rows();
      if (table.GroupBy != null)
      {
        foreach (var group in rows.GroupBy(table.GroupBy))
        {
          yield return factory.Create(group.Rollup().ToArray());
        }
      }
      else
      {
        foreach (var row in rows)
        {
          yield return factory.Create(row);
        }
      }
    }
  }

  public static class ColumnHelpers
  {
    public static IEnumerable<object> Rollup(this IEnumerable<object[]> rows)
    {
      var arrayOfRows = rows.ToArray();
      if (arrayOfRows.Length == 0)
      {
        yield break;
      }
      for (var i = 0; i < arrayOfRows[0].Length; ++i)
      {
        var column = rows.Select(x => x[i]);
        if (column.Distinct().Count() == 1)
        {
          yield return column.First();
        }
        else
        {
          yield return column.ToArray();
        }
      }
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