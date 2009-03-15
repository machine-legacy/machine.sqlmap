using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.SqlMap
{
  public class ColumnAndTable
  {
    readonly Table _table;
    readonly Column _column;
    readonly IProjectedTable _projectedTable;

    public Table Table
    {
      get { return _table; }
    }

    public Column Column
    {
      get { return _column; }
    }

    public IProjectedTable ProjectedTable
    {
      get { return _projectedTable; }
    }

    public ColumnAndTable(Table table, Column column, IProjectedTable projectedTable)
    {
      _table = table;
      _projectedTable = projectedTable;
      _column = column;
    }
  }

  public class MappableType
  {
    readonly TypeConstructor[] _constructors;

    public TypeConstructor[] Constructors
    {
      get { return _constructors; }
    }

    public MappableType(TypeConstructor[] constructors)
    {
      _constructors = constructors;
    }

    public MappedConstructor MapToConstructor(IEnumerable<ColumnAndTable> columnsAndTables)
    {
      var error = new ErrorBuilder();
      var columnsByName = columnsAndTables.ToDictionary(x => x.Column.Name.ToUpper());
      foreach (TypeConstructor ctor in _constructors)
      {
        var columns = new List<ColumnAndTable>();
        foreach (var attribute in ctor.Attributes)
        {
          var key = attribute.Name.ToUpper();
          if (!columnsByName.ContainsKey(key))
          {
            error.UnmappedAttribute(attribute);
          }
          else
          {
            columns.Add(columnsByName[key]);
          }
        }
        if (!error.HasErrors)
        {
          return new MappedConstructor(ctor, columns);
        }
      }
      throw error.Create();
    }

    public static MappableType For(Type type)
    {
      var ctors = type.GetConstructors().Select(
        ctor => new TypeConstructor(ctor,
          ctor.GetParameters().Select(x => new Attribute(x.Name, x.ParameterType)).ToArray()
        )
      );
      return new MappableType(ctors.ToArray());
    }
  }

  public class ErrorBuilder
  {
    readonly List<Attribute> _unmapped = new List<Attribute>();

    public void UnmappedAttribute(Attribute attribute)
    {
      _unmapped.Add(attribute);
    }

    public bool HasErrors
    {
      get { return _unmapped.Any(); }
    }

    public SqlMapException Create()
    {
      string unmapped = "Unmapped Attributes:\n" + _unmapped.Select(x => x.Type +" " + x.Name).Aggregate("", (a, x) => a += x + "\n");
      return new SqlMapException(unmapped.Trim());
    }
  }
}