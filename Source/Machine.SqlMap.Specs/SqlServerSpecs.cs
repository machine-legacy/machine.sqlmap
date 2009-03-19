using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

using Machine.Specifications;

namespace Machine.SqlMap
{
  public class with_database_connection
  {
    protected static IDbConnection connection;

    Establish context = () =>
    {
      connection = new SqlConnection("Server=127.0.0.1;Initial Catalog=Northwind;Integrated Security=true");
      connection.Open();
      connection.ExecuteUpdateQuery("TRUNCATE TABLE Employees", (c) => { });
      connection.ExecuteUpdateQuery("INSERT INTO Employees (FirstName, LastName, BirthDate, IsAlive, ReportsTo) VALUES (@firstName, @lastName, @birthDate, 1, NULL)", (c) => {
        c.AddParameter("firstName", "Jacob");
        c.AddParameter("lastName", "Lewallen");
        c.AddParameter("birthDate", DateTime.Parse("4/23/1982 9:45AM"));
      });
      connection.ExecuteUpdateQuery("INSERT INTO Employees (FirstName, LastName, BirthDate, IsAlive, ReportsTo) VALUES (@firstName, @lastName, @birthDate, 1, NULL)", (c) => {
        c.AddParameter("firstName", "Andrea");
        c.AddParameter("lastName", "Lewallen");
        c.AddParameter("birthDate", DateTime.Parse("11/14/1985 9:45AM"));
      });
    };

    Cleanup after = () =>
    {
      connection.Dispose();
    };
  }

  [Subject("ADO.NET Integration")]
  public class when_querying_for_one_type : with_database_connection
  {
    static Person[] people;

    Because of = () =>
    {
      var query = connection.CreateQuery();
      people = query.Add<Person>("SELECT EmployeeId AS Id, FirstName, LastName, BirthDate FROM Employees").ToEnumerable().ToArray();
    };

    It should_return_a_person_for_each_row = () =>
      people.Count().ShouldEqual(2);
  }

  [Subject("ADO.NET Integration")]
  public class when_querying_for_two_types : with_database_connection
  {
    static Person[] first;
    static Person[] second;

    Because of = () =>
    {
      var query = connection.CreateQuery();
      var firstQuery = query.Add<Person>("SELECT EmployeeId AS Id, FirstName, LastName, BirthDate FROM Employees");
      var secondQuery = query.Add<Person>("SELECT EmployeeId AS Id, FirstName, LastName, BirthDate FROM Employees");
      first = firstQuery.ToEnumerable().ToArray();
      second = secondQuery.ToEnumerable().ToArray();
    };

    It should_return_a_person_for_each_row_in_the_first_query = () =>
      first.Count().ShouldEqual(2);

    It should_return_a_person_for_each_row_in_the_second_query = () =>
      second.Count().ShouldEqual(2);
  }

  [Subject("ADO.NET Integration")]
  public class when_adding_queries_after_a_its_fetched : with_database_connection
  {
    static Exception error;

    Because of = () =>
    {
      var query = connection.CreateQuery();
      var people = query.Add<Person>("SELECT EmployeeId AS Id, FirstName, LastName, BirthDate FROM Employees").ToEnumerable();
      error = Catch.Exception(() => query.Add<Person>("SELECT EmployeeId AS Id, FirstName, LastName, BirthDate FROM Employees"));
    };

    It should_throw = () =>
      error.ShouldNotBeNull();
  }

  public class Order
  {
    readonly Int32 _id;
    readonly string _title;

    public Int32 Id
    {
      get { return _id; }
    }

    public string Title
    {
      get { return _title; }
    }

    public Order(Int32 id, string title)
    {
      _id = id;
      _title = title;
    }
  }

  public class Person
  {
    readonly Int32 _id;
    readonly string _firstName;

    public Int32 Id
    {
      get { return _id; }
    }

    public string FirstName
    {
      get { return _firstName; }
    }

    public Person(Int32 id, string firstName)
    {
      _id = id;
      _firstName = firstName;
    }
  }
}
