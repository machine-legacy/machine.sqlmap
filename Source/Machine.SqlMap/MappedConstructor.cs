using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.SqlMap
{
  public class MappedConstructor
  {
    readonly TypeConstructor _typeConstructor;
    readonly Column[] _columns;

    public MappedConstructor(TypeConstructor typeConstructor, IEnumerable<Column> columns)
    {
      _typeConstructor = typeConstructor;
      _columns = columns.ToArray();
    }

    public Factory ToFactory(TypeMapper typeMapper)
    {
      return _typeConstructor.ToFactory(_columns, typeMapper);
    }
  }
}