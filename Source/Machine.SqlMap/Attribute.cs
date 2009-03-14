using System;
using System.Collections.Generic;

namespace Machine.SqlMap
{
  public class Attribute
  {
    readonly string _name;
    readonly Type _type;

    public string Name
    {
      get { return _name; }
    }

    public Type Type
    {
      get { return _type; }
    }

    public Attribute(string name, Type type)
    {
      _name = name;
      _type = type;
    }

    public override string ToString()
    {
      return "Attribute<" + _name + ", " + _type + ">";
    }
  }
}