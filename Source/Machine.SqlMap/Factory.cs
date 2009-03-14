using System;
using System.Collections.Generic;
using System.Reflection;

namespace Machine.SqlMap
{
  public class Factory
  {
    readonly ConstructorInfo _info;
    readonly Func<object[], object>[] _readers;

    public Factory(ConstructorInfo info, Func<object[], object>[] readers)
    {
      _info = info;
      _readers = readers;
    }

    public object Create(object[] values)
    {
      List<object> parameters = new List<object>();
      foreach (var reader in _readers)
      {
        parameters.Add(reader(values));
      }
      return _info.Invoke(parameters.ToArray());
    }
  }
}