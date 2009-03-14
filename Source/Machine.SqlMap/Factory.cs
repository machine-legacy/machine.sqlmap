using System;
using System.Collections.Generic;
using System.Reflection;

namespace Machine.SqlMap
{
  public class Factory
  {
    readonly ConstructorInfo _info;
    readonly Func<object[], object>[] _columnReaders;

    public Factory(ConstructorInfo info, Func<object[], object>[] columnReaders)
    {
      _info = info;
      _columnReaders = columnReaders;
    }

    public object Create(object[] values)
    {
      List<object> parameters = new List<object>();
      foreach (var reader in _columnReaders)
      {
        parameters.Add(reader(values));
      }
      return _info.Invoke(parameters.ToArray());
    }
  }
}