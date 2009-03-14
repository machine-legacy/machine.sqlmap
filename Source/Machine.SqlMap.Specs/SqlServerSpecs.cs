using System;
using System.Data.SqlClient;

namespace Machine.SqlMap
{
  public class SqlQueries
  {
    public void Run()
    {
      using (var connection = new SqlConnection("Server=127.0.0.1;Initial Catalog=eldb01;Integrated Security=true"))
      {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"SELECT TOP 10 * FROM el_user"; // ; SELECT * FROM el_session_type
        using (var reader = command.ExecuteReader())
        {
          if (reader == null)
          {
            throw new InvalidOperationException();
          }
          do
          {
            //SqlMapper mapper = new SqlMapper();
            //foreach (var person in mapper.Map<Person>(new ReaderTable(reader)))
            //{
              //Console.WriteLine(person);
            //}
          }
          while (reader.NextResult());
        }
      }
    }
  }
}
