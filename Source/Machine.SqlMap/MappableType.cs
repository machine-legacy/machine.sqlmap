using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.SqlMap
{
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

    public MappedConstructor MapToConstructor(IEnumerable<Column> columns)
    {
      var error = new ErrorBuilder();
      var columnsByName = columns.ToDictionary(x => x.Name.ToUpper());
      foreach (TypeConstructor ctor in _constructors)
      {
        var selectedColumns = new List<Column>();
        foreach (var attribute in ctor.Attributes)
        {
          var key = attribute.Name.ToUpper();
          if (!columnsByName.ContainsKey(key))
          {
            error.UnmappedAttribute(attribute);
          }
          else
          {
            selectedColumns.Add(columnsByName[key]);
          }
        }
        if (!error.HasErrors)
        {
          return new MappedConstructor(ctor, selectedColumns);
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