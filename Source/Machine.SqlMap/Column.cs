using System;
using System.Collections.Generic;

namespace Machine.SqlMap
{
  public class Column
  {
    readonly string _name;
    readonly Int32 _ordinal;
    readonly Type _type;
    readonly Func<object[], object> _reader;

    public string Name
    {
      get { return _name; }
    }

    public Int32 Ordinal
    {
      get { return _ordinal; }
    }

    public Type Type
    {
      get { return _type; }
    }

    public Func<object[], object> Read
    {
      get { return _reader; }
    }

    public Column(string name, Int32 ordinal, Type type, Func<object, object> map)
    {
      _name = name;
      _ordinal = ordinal;
      _type = type;
      _reader = (row) => map(row[_ordinal]);
    }

    public Column(string name, Int32 ordinal, Type type)
    {
      _name = name;
      _ordinal = ordinal;
      _type = type;
      _reader = (row) => row[_ordinal];
    }

    public override string ToString()
    {
      return "Column<" + _name + ", " + _ordinal + ", " + _type + ">";
    }
  }
}