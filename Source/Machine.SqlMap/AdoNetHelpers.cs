using System;
using System.Data;

namespace Machine.SqlMap
{
  public static class AdoNetHelpers
  {
    public static object[] ToArray(this IDataReader reader)
    {
      object[] values = new object[reader.FieldCount];
      reader.GetValues(values);
      return values;
    }
  }
}