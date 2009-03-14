using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.SqlMap
{
  public class StaticTable : IProjectedTable
  {
    readonly Table _table;
    readonly List<object[]> _rows = new List<object[]>();

    public StaticTable(Table table, params object[][] rows)
    {
      _table = table;
      _rows = rows.ToList();
    }

    public Table ToTable()
    {
      return _table;
    }

    public IEnumerable<object[]> Rows()
    {
      return _rows;
    }
  }
}