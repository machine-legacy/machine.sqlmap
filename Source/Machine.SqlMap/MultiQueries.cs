using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Machine.SqlMap
{
  public static class MultiQueryHelpers
  {
    public static MappedSqlMultiQuery CreateQuery(this IDbConnection connection)
    {
      return new MappedSqlMultiQuery(connection);
    }
  }

  public class SqlCollector
  {
    readonly List<string> _queries = new List<string>();
    
    public void Add(string query)
    {
      _queries.Add(query);
    }

    public string ToSingleQuery()
    {
      return _queries.Aggregate(String.Empty, (a, f) => a + Environment.NewLine + f).Trim();
    }
  }

  public class MappedSqlMultiQuery
  {
    readonly IDbConnection _connection;
    readonly List<ISubQuery> _queries = new List<ISubQuery>();
    readonly SqlCollector _sqlCollector = new SqlCollector();
    readonly TypeMapper _typeMapper;
    readonly SqlMapper _sqlMapper;
    bool _read;

    public MappedSqlMultiQuery(IDbConnection connection)
    {
      _connection = connection;
      _typeMapper = new TypeMapper();
      _sqlMapper = new SqlMapper(_typeMapper);
    }

    public SqlQuery<T> Add<T>(string sql)
    {
      if (_read)
      {
        throw new InvalidOperationException("Error, already fetched this multiquery!");
      }
      _sqlCollector.Add("/*" + typeof(T) + "*/ " + sql + ";");
      SqlQuery<T> query = new SqlQuery<T>(this);
      _queries.Add(query);
      return query;
    }

    public void ReadIfNecessary()
    {
      if (_read)
      {
        return;
      }
      _read = true;
      CreateAndExecute();
    }

    public void CreateAndExecute()
    {
      IDbCommand command = _connection.CreateCommand();
      command.CommandText = _sqlCollector.ToSingleQuery();
      IEnumerator<ISubQuery> enumerator = _queries.GetEnumerator();
      using (IDataReader reader = command.ExecuteReader())
      {
        do
        {
          enumerator.MoveNext();
          enumerator.Current.Read(_typeMapper, _sqlMapper, reader);
        }
        while (reader.NextResult());
      }
    }
  }

  public interface ISubQuery
  {
    void Read(TypeMapper typeMapper, SqlMapper sqlMapper, IDataReader reader);
  }

  public class SqlQuery<T> : ISubQuery
  {
    readonly MappedSqlMultiQuery _query;
    IEnumerable<T> _values;

    public SqlQuery(MappedSqlMultiQuery query)
    {
      _query = query;
    }

    void ISubQuery.Read(TypeMapper typeMapper, SqlMapper sqlMapper, IDataReader reader)
    {
      _values = sqlMapper.Map<T>(new ReaderTable(reader)).ToArray();
    }

    public IEnumerable<T> ToEnumerable()
    {
      _query.ReadIfNecessary();
      return _values;
    }

    public T ToUnique()
    {
      _query.ReadIfNecessary();
      return _values.SingleOrDefault();
    }
  }
}
