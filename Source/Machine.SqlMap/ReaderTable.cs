using System;
using System.Collections.Generic;
using System.Data;

namespace Machine.SqlMap
{
  public class ReaderTable : IProjectedTable
  {
    readonly IDataReader _reader;

    public ReaderTable(IDataReader reader)
    {
      _reader = reader;
    }

    public Table ToTable()
    {
      List<Column> columns = new List<Column>();
      for (var i = 0; i < _reader.FieldCount; ++i)
      {
        columns.Add(new Column(_reader.GetName(i), i, _reader.GetFieldType(i)));
      }
      return new Table(columns.ToArray());
    }

    public IEnumerable<object[]> Rows()
    {
      while (_reader.Read())
      {
        yield return _reader.ToArray();
      }
    }
  }
}