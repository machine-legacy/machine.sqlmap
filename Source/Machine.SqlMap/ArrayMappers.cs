using System;

namespace Machine.SqlMap
{
  public static class ArrayMappers
  {
    public static Array Select<S>(this Array array, Type destinyType, Func<S, object> func)
    {
      var destiny = Array.CreateInstance(destinyType, array.Length);
      for (var i = 0; i < array.Length; ++i)
      {
        destiny.SetValue(func((S)array.GetValue(i)), i);
      }
      return destiny; 
    }

    public static Array CastOrWrapInArray(this object obj)
    {
      if (obj is Array) return (Array)obj;
      var objectArray = Array.CreateInstance(typeof (Object), 1);
      objectArray.SetValue(obj, 0);
      return objectArray;
    }
  }
}