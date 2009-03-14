using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Machine.SqlMap
{
  public class TypeConstructor
  {
    readonly ConstructorInfo _info;
    readonly Attribute[] _attributes;

    public Attribute[] Attributes
    {
      get { return _attributes; }
    }

    public TypeConstructor(ConstructorInfo info, Attribute[] attributes)
    {
      _info = info;
      _attributes = attributes;
    }

    public Factory ToFactory(IEnumerable<ColumnAndTable> columns)
    {
      if (columns.Count() != _attributes.Count())
      {
        throw new InvalidOperationException("Did not get equal number of Columns and Attributes");
      }
      var readers = columns.Zip<ColumnAndTable, Attribute, Func<object[], object>>(_attributes, (c, a) => {
        var column = c.Column;
        if (typeof(DateTimeOffset) == a.Type && typeof(DateTime) == column.Type)
        {
          return (row) => new DateTimeOffset((DateTime)column.Read(row));
        }
        if (typeof(TimeSpan) == a.Type && typeof(Int64) == column.Type)
        {
          return (row) => new TimeSpan((Int64)column.Read(row));
        }
        if (a.Type.IsArray && !column.Type.IsArray)
        {
          return (row) => {
            var array = Array.CreateInstance(column.Type, 1);
            array.SetValue(column.Read(row), 0);
            return array;
          };
        }
        return column.Read;
      });
      return new Factory(_info, readers.ToArray());
    }
  }
}