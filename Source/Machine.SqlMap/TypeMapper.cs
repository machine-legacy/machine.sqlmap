using System;
using System.Collections.Generic;

namespace Machine.SqlMap
{
  public class TypeMapper
  {
    readonly Dictionary<MappingKey, Func<object, object>> _mappings = new Dictionary<MappingKey, Func<object, object>>();

    public TypeMapper()
    {
      _mappings[MappingKey.For<DateTime, DateTimeOffset>()] = (value) => new DateTimeOffset((DateTime)value);
      _mappings[MappingKey.For<Int64, TimeSpan>()] = (value) => new TimeSpan((Int64)value);
    }

    public void Map<K, V>(Func<K, V> map)
    {
      _mappings[MappingKey.For<K, V>()] = (k) => map((K)k);
    }

    public void Map<K, V>(IDictionary<K, V> map)
    {
      _mappings[MappingKey.For<K, V>()] = (k) => map[(K)k];
    }

    public Func<object, object> MappingFor(Column column, Attribute attribute)
    {
      return MapTypesAndArrays(column, attribute);
    }

    private Func<object, object> MapTypesAndArrays(Column column, Attribute attribute)
    {
      var typeMapper = MapTypes(column, attribute);
      if (!attribute.Type.IsArray)
      {
        return typeMapper;
      }
      return (source) => source.CastOrWrapInArray().Select(attribute.Type.GetElementType(), typeMapper);
    }

    private Func<object, object> MapTypes(Column column, Attribute attribute)
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

    protected Type AttributeTypeOrItsElementType
    {
      get
      {
        if (_attributeType.HasElementType)
        {
          return _attributeType.GetElementType();
        }
        return _attributeType;
      }
    }

    public MappingKey(Type columnType, Type attributeType)
    {
      _columnType = columnType;
      _attributeType = attributeType;
    }

    public bool Equals(MappingKey other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other._columnType, _columnType) && Equals(other.AttributeTypeOrItsElementType, AttributeTypeOrItsElementType);
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
      return _columnType.GetHashCode() ^ AttributeTypeOrItsElementType.GetHashCode();
    }

    public static MappingKey For<C, A>()
    {
      return new MappingKey(typeof(C), typeof(A));
    }
  }
}