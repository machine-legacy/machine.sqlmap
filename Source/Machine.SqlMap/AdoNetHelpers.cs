using System;
using System.Data;
using System.Linq;

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

    public static int ExecuteUpdateQuery(this IDbConnection connection, string sql, Action<IDbCommand> prepare)
    {
      var command = connection.CreateCommand();
      command.CommandText = sql;
      prepare(command);
      return command.ExecuteNonQuery();
    }

    public static void AddParameter(this IDbCommand command, string name, object value)
    {
      DbType type;
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      else if (value is String) type = DbType.String;
      else if (value is Int32) type = DbType.Int32;
      else if (value is Int16) type = DbType.Int16;
      else if (value is DateTime) type = DbType.DateTime;
      else if (value is DateTimeOffset)
      {
        type = DbType.DateTime;
        value = ((DateTimeOffset)value).ToUniversalTime().DateTime;
      }
      else if (value is Int32[])
      {
        type = DbType.String;
        value = String.Join(",", ((Int32[])value).Select(x => x.ToString()).ToArray());
      }
      else if (value is Guid)
      {
        type = DbType.Guid;
      }
      else throw new ArgumentException("value");
      command.CreateParameter(name, type).Value = value;
    }

    public static void AddParameter(this IDbCommand command, string name, DbType dbType, object value)
    {
      command.CreateParameter(name, dbType).Value = value;
    }

    public static IDbDataParameter CreateParameter(this IDbCommand command, string name, DbType dbType)
    {
      IDbDataParameter parameter = command.CreateParameter();
      parameter.ParameterName = name;
      parameter.DbType = dbType;
      command.Parameters.Add(parameter);
      return parameter;
    }
  }
}