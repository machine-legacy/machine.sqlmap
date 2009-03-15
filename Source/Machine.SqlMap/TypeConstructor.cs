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

    public Factory ToFactory(IEnumerable<ColumnAndTable> columns, TypeMapper typeMapper)
    {
      if (columns.Count() != _attributes.Count())
      {
        throw new InvalidOperationException("Did not get equal number of Columns and Attributes");
      }
      var readers = columns.Zip<ColumnAndTable, Attribute, Func<object[], object>>(_attributes, (c, attribute) => {
        var mapper = typeMapper.MappingFor(c.Column, attribute);
        return (row) => mapper(c.Column.Read(row));
      });
      return new Factory(_info, readers.ToArray());
    }
  }
}