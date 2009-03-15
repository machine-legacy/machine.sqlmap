using System;
using System.Collections.Generic;

namespace Machine.SqlMap
{
  public static class TypeMapping
  {
    static readonly Dictionary<MappingKey, Func<object, object>> _mappings = new Dictionary<MappingKey, Func<object, object>>();

    static TypeMapping()
    {
      _mappings[MappingKey.For<DateTime, DateTimeOffset>()] = (value) => new DateTimeOffset((DateTime)value);
      _mappings[MappingKey.For<Int64, TimeSpan>()] = (value) => new TimeSpan((Int64)value);
    }

    public static Func<object, object> MappingFor(Column column, Attribute attribute)
    {
      var mapTypes = MapTypes(column, attribute);
      var mapArrays = MapArrays(column, attribute);
      return (value) => mapArrays(mapTypes(value));
    }

    private static Func<object, object> MapArrays(Column column, Attribute attribute)
    {
      if (!attribute.Type.IsArray)
      {
        return (value) => value;
      }
      return (source) =>
      {
        return ((Array)source).Select<object>(attribute.Type.GetElementType(), (value) => value);
      };
    }

    private static Func<object, object> MapTypes(Column column, Attribute attribute)
    {
      MappingKey key = new MappingKey(column.Type, attribute.Type);
      if (_mappings.ContainsKey(key))
      {
        return _mappings[key];
      }
      return (value) => value;
    }
  }

  public class MappingKey
  {
    readonly Type _columnType;
    readonly Type _attributeType;

    public MappingKey(Type columnType, Type attributeType)
    {
      _columnType = columnType;
      _attributeType = attributeType;
    }

    public bool Equals(MappingKey other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other._columnType, _columnType) && Equals(other._attributeType, _attributeType);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(MappingKey)) return false;
      return Equals((MappingKey)obj);
    }

    public override Int32 GetHashCode()
    {
      return _columnType.GetHashCode() ^ _attributeType.GetHashCode();
    }

    public static MappingKey For<C, A>()
    {
      return new MappingKey(typeof(C), typeof(A));
    }
  }
}