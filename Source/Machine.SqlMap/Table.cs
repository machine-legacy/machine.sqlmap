using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.SqlMap
{
  public class Table
  {
    readonly List<Column> _columns;

    public IEnumerable<Column> Columns
    {
      get { return _columns; }
    }

    public Table(params Column[] columns)
    {
      _columns = columns.OrderBy(x => x.Ordinal).ToList();
    }

    public Column MapColumn<K, V>(string originalName, string newName, IDictionary<K, V> map)
    {
      Column original = FindByName(originalName);
      Column column = new Column(newName, original.Ordinal, typeof(V), (k) => map[(K)k]);
      _columns.Add(column);
      return column;
    }

    public Column FindByName(string name)
    {
      return _columns.Where(x => x.Name.Equals(name)).Single();
    }
  }
}