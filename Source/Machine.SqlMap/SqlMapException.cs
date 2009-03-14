using System;
using System.Runtime.Serialization;

namespace Machine.SqlMap
{
  public class SqlMapException : Exception
  {
    public SqlMapException()
    {
    }

    public SqlMapException(string message) : base(message)
    {
    }

    public SqlMapException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SqlMapException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}