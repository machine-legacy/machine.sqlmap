using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.SqlMap
{
  public class Table
  {
    readonly List<Column> _columns;
    Func<object[], object> _groupBy;

    public Func<object[], object> GroupBy
    {
      get { return _groupBy; }
      set { _groupBy = value; }
    }

    public Table(params Column[] columns)
    {
      _columns = columns.OrderBy(x => x.Ordinal).ToList();
    }

    public void RenameColumn(string originalName, string newName)
    {
      Column original = FindByName(originalName);
      Column column = new Column(newName, original.Ordinal, original.Type);
      _columns.Add(column);
    }

    public Column MapColumn<K, V>(string originalName, string newName, IDictionary<K, V> map)
    {
      Column original = FindByName(originalName);
      Column column = new Column(newName, original.Ordinal, typeof(V), (k) => {
        if (k is Array)
        {
          return ((Array)k).Select<K>(typeof (V), (value) => {
            return map[value];
          });
        }
        if (k is K) return map[(K)k];
        throw new SqlMapException("Not casting " + k + " to " + typeof(K) + " for " + originalName + " to " + newName);
      });
      _columns.Add(column);
      return column;
    }

    public Column FindByName(string name)
    {
      return _columns.Where(x => x.Name.Equals(name)).Single();
    }

    public IEnumerable<ColumnAndTable> ToColumnsAndTables(IProjectedTable projected)
    {
      return _columns.Select(c => new ColumnAndTable(this, c, projected));
    }
  }
}