using System;
using System.Collections.Generic;
using System.Linq;

using Machine.Specifications;

namespace Machine.SqlMap.Specs
{
  public class SqlProjectorSpecs
  {
    protected static SqlMapper mapper = new SqlMapper();
    protected static Column[] columns;
    protected static object[][] rows;
    protected static StaticTable table;
    protected static Exception exception;
  }

  public class with_table_that_has_first_name_and_last_name : SqlProjectorSpecs
  {
    Establish context = () =>
    {
      columns = new[] {
        new Column("FirstName", 0, typeof(string)),
        new Column("LastName", 1, typeof(string))
      };
      rows = new[] {
        new[] { "Jacob", "Lewallen" },
        new[] { "Andrea", "Tabuenca" }
      };
    };
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_no_constructors : with_table_that_has_first_name_and_last_name
  {
    static IEnumerable<NoConstructor> mapped;

    Because of = () =>
      mapped = mapper.Map<NoConstructor>(new Table(columns), rows);

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);
  }

  public class NoConstructor
  {
  }

  [Subject("Sql Projector")]
  public class with_type_that_throws_during_construction : with_table_that_has_first_name_and_last_name
  {
    Because of = () =>
      exception = Catch.Exception(() => mapper.Map<HasUserId>(new Table(columns), rows).ToArray());

    It should_throw_descriptive_exception = () =>
      exception.ShouldNotBeNull();
  }

  public class ThrowsDuringConstruction
  {
    public ThrowsDuringConstruction()
    {
      throw new ArgumentException();
    }
  }

  [Subject("Sql Projector")]
  public class with_type_that_requires_attribute_that_is_missing_from_table : with_table_that_has_first_name_and_last_name
  {
    Because of = () =>
      exception = Catch.Exception(() => mapper.Map<HasUserId>(new Table(columns), rows).ToArray());

    It should_throw_descriptive_exception = () =>
      exception.ShouldNotBeNull();
  }

  public class HasUserId
  {
    readonly Guid _userId;

    public Guid UserId
    {
      get { return _userId; }
    }

    public HasUserId(Guid userId)
    {
      _userId = userId;
    }
  }

  public class with_table_that_has_user_id_first_name_and_last_name : SqlProjectorSpecs
  {
    Establish context = () =>
    {
      columns = new[] {
        new Column("UserId", 0, typeof(Guid)),
        new Column("FirstName", 1, typeof(string)),
        new Column("LastName", 2, typeof(string))
      };
      rows = new [] {
        new object[] { Guid.NewGuid(), "Jacob", "Lewallen" },
        new object[] { Guid.NewGuid(), "Andrea", "Tabuenca" }
      };
    };
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_single_guid_attribute : with_table_that_has_user_id_first_name_and_last_name
  {
    static IEnumerable<HasUserId> mapped;

    Because of = () =>
      mapped = mapper.Map<HasUserId>(new Table(columns), rows).ToArray();

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);

    It should_set_first_instances_attributes_to_first_rows_value = () =>
      mapped.First().UserId.ShouldEqual(rows[0][0]);

    It should_set_second_instances_attributes_to_second_rows_value = () =>
      mapped.Last().UserId.ShouldEqual(rows[1][0]);
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_single_two_string_attributes : with_table_that_has_user_id_first_name_and_last_name
  {
    static IEnumerable<TwoStringAttributes> mapped;

    Because of = () =>
      mapped = mapper.Map<TwoStringAttributes>(new Table(columns), rows).ToArray();

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);

    It should_set_first_instances_attributes_to_first_rows_second_columns_value = () =>
      mapped.First().FirstName.ShouldEqual(rows[0][1]);

    It should_set_second_instances_attributes_to_second_rows_second_columns_value = () =>
      mapped.Last().FirstName.ShouldEqual(rows[1][1]);

    It should_set_first_instances_attributes_to_first_rows_third_columns_value = () =>
      mapped.First().LastName.ShouldEqual(rows[0][2]);

    It should_set_second_instances_attributes_to_second_rows_third_columns_value = () =>
      mapped.Last().LastName.ShouldEqual(rows[1][2]);
  }

  public class TwoStringAttributes
  {
    readonly string _firstName;
    readonly string _lastName;

    public string FirstName
    {
      get { return _firstName; }
    }

    public string LastName
    {
      get { return _lastName; }
    }

    public TwoStringAttributes(string firstName, string lastName)
    {
      _firstName = firstName;
      _lastName = lastName;
    }
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_date_time_attribute : SqlProjectorSpecs
  {
    static IEnumerable<HasDateTimeAttribute> mapped;

    Establish context = () =>
    {
      columns = new[] {
        new Column("UserId", 0, typeof(Guid)),
        new Column("Name", 1, typeof(string)),
        new Column("ScheduledAt", 2, typeof(DateTime))
      };
      rows = new [] {
        new object[] { Guid.NewGuid(), "Jacob", DateTime.Today },
        new object[] { Guid.NewGuid(), "Andrea", DateTime.Today.AddDays(1.0) }
      };
    };
    
    Because of = () =>
      mapped = mapper.Map<HasDateTimeAttribute>(new Table(columns), rows).ToArray();

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);

    It should_set_first_instances_attributes_to_first_rows_value = () =>
      mapped.First().ScheduledAt.ShouldEqual(rows[0][2]);

    It should_set_second_instances_attributes_to_second_rows_value = () =>
      mapped.Last().ScheduledAt.ShouldEqual(rows[1][2]);
  }

  public class HasDateTimeAttribute
  {
    readonly DateTime _scheduledAt;

    public DateTime ScheduledAt
    {
      get { return _scheduledAt; }
    }

    public HasDateTimeAttribute(DateTime scheduledAt)
    {
      _scheduledAt = scheduledAt;
    }
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_date_time_offset_attribute : SqlProjectorSpecs
  {
    static IEnumerable<HasDateTimeOffsetAttribute> mapped;

    Establish context = () =>
    {
      columns = new[] {
        new Column("UserId", 0, typeof(Guid)),
        new Column("Name", 1, typeof(string)),
        new Column("ScheduledAt", 2, typeof(DateTime))
      };
      rows = new [] {
        new object[] { Guid.NewGuid(), "Jacob", DateTime.Today },
        new object[] { Guid.NewGuid(), "Andrea", DateTime.Today.AddDays(1.0) }
      };
    };
    
    Because of = () =>
      mapped = mapper.Map<HasDateTimeOffsetAttribute>(new Table(columns), rows).ToArray();

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);

    It should_set_first_instances_attributes_to_first_rows_value = () =>
      mapped.First().ScheduledAt.ShouldEqual(new DateTimeOffset((DateTime)rows[0][2]));

    It should_set_second_instances_attributes_to_second_rows_value = () =>
      mapped.Last().ScheduledAt.ShouldEqual(new DateTimeOffset((DateTime)rows[1][2]));
  }

  public class HasDateTimeOffsetAttribute
  {
    readonly DateTimeOffset _scheduledAt;

    public DateTimeOffset ScheduledAt
    {
      get { return _scheduledAt; }
    }

    public HasDateTimeOffsetAttribute(DateTimeOffset scheduledAt)
    {
      _scheduledAt = scheduledAt;
    }
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_enum_attribute : SqlProjectorSpecs
  {
    static IEnumerable<HasEnumAttribute> mapped;

    Establish context = () =>
    {
      columns = new[] {
        new Column("UserId", 0, typeof(Guid)),
        new Column("Useful", 1, typeof(DateTime))
      };
      rows = new [] {
        new object[] { Guid.NewGuid(), 1 },
        new object[] { Guid.NewGuid(), 2 }
      };
    };
    
    Because of = () =>
      mapped = mapper.Map<HasEnumAttribute>(new Table(columns), rows).ToArray();

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);

    It should_set_first_instances_attributes_to_first_rows_value = () =>
      mapped.First().Useful.ShouldEqual(ThisIsUseful.No);

    It should_set_second_instances_attributes_to_second_rows_value = () =>
      mapped.Last().Useful.ShouldEqual(ThisIsUseful.Maybe);
  }

  public class HasEnumAttribute
  {
    readonly ThisIsUseful _useful;

    public ThisIsUseful Useful
    {
      get { return _useful; }
    }

    public HasEnumAttribute(ThisIsUseful useful)
    {
      _useful = useful;
    }
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_time_span_attribute : SqlProjectorSpecs
  {
    static IEnumerable<HasTimeSpanAttribute> mapped;

    Establish context = () =>
    {
      columns = new[] {
        new Column("UserId", 0, typeof(Guid)),
        new Column("Interval", 1, typeof(Int64))
      };
      rows = new [] {
        new object[] { Guid.NewGuid(), TimeSpan.FromMinutes(2).Ticks },
        new object[] { Guid.NewGuid(), TimeSpan.FromMinutes(1).Ticks }
      };
    };
    
    Because of = () =>
      mapped = mapper.Map<HasTimeSpanAttribute>(new Table(columns), rows).ToArray();

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);

    It should_set_first_instances_attributes_to_first_rows_value = () =>
      mapped.First().Interval.ShouldEqual(TimeSpan.FromMinutes(2));

    It should_set_second_instances_attributes_to_second_rows_value = () =>
      mapped.Last().Interval.ShouldEqual(TimeSpan.FromMinutes(1));
  }

  public class HasTimeSpanAttribute
  {
    readonly TimeSpan _interval;

    public TimeSpan Interval
    {
      get { return _interval; }
    }

    public HasTimeSpanAttribute(TimeSpan interval)
    {
      _interval = interval;
    }
  }

  public enum ThisIsUseful
  {
    Yes,
    No,
    Maybe
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_one_to_one_foreign_key_to_another_type : SqlProjectorSpecs
  {
    static ParentType[] mapped;
    static ChildType[] children;

    Establish context = () =>
    {
      columns = new[] {
        new Column("Name", 0, typeof(string)),
        new Column("ChildId", 1, typeof(Int32))
      };
      rows = new [] {
        new object[] { "A", 1 },
        new object[] { "B", 1 },
        new object[] { "C", 2 }
      };
      children = new[] {
        new ChildType(1, "Jacob"), 
        new ChildType(2, "Andrea"), 
      };
    };

    Because of = () =>
    {
      Table table = new Table(columns);
      table.MapColumn("ChildId", "Child", children.ToDictionary(x => x.Id));
      mapped = mapper.Map<ParentType>(table, rows).ToArray();
    };

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(3);

    It should_set_first_instances_attributes_to_first_rows_first_columns_value = () =>
      mapped[0].Name.ShouldEqual("A");

    It should_set_second_instances_attributes_to_second_rows_first_columns_value = () =>
      mapped[1].Name.ShouldEqual("B");

    It should_set_third_instances_attributes_to_third_rows_first_columns_value = () =>
      mapped[2].Name.ShouldEqual("C");

    It should_set_first_instances_attributes_to_first_rows_second_columns_value = () =>
      mapped[0].Child.ShouldEqual(children[0]);

    It should_set_second_instances_attributes_to_second_rows_second_columns_value = () =>
      mapped[1].Child.ShouldEqual(children[0]);

    It should_set_third_instances_attributes_to_third_rows_second_columns_value = () =>
      mapped[2].Child.ShouldEqual(children[1]);
  }

  [Subject("Sql Projector")]
  public class with_type_unique_on_a_key : SqlProjectorSpecs
  {
    static ChildType[] mapped;

    Establish context = () =>
    {
      columns = new[] {
        new Column("Id", 0, typeof(Int32)),
        new Column("Name", 1, typeof(string))
      };
      rows = new [] {
        new object[] { 1, "A" },
        new object[] { 1, "A" },
        new object[] { 2, "C" },
        new object[] { 2, "C" },
        new object[] { 2, "C" }
      };
    };
    
    Because of = () =>
      mapped = mapper.Map<ChildType>(new Table(columns), rows).UniqueOn(x => x.Id).ToArray();

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);

    It should_set_first_instances_name = () =>
      mapped[0].Name.ShouldEqual("A");

    It should_set_second_instances_name = () =>
      mapped[1].Name.ShouldEqual("C");
  }

  [Subject("Sql Projector")]
  public class with_type_that_has_one_to_many_foreign_key_to_another_type : SqlProjectorSpecs
  {
    static OneToManyParentType[] mapped;
    static ChildType[] children;

    Establish context = () =>
    {
      columns = new[] {
        new Column("Id", 0, typeof(Int32)),
        new Column("Name", 1, typeof(string)),
        new Column("ChildId", 2, typeof(Int32))
      };
      rows = new [] {
        new object[] { 1, "A", 1 },
        new object[] { 1, "A", 2 },
        new object[] { 2, "C", 1 },
        new object[] { 2, "C", 2 },
        new object[] { 2, "C", 3 }
      };
      children = new[] {
        new ChildType(1, "Jacob"), 
        new ChildType(2, "Andrea"), 
        new ChildType(3, "Tomorrow"), 
      };
    };
    
    Because of = () =>
    {
      Table table = new Table(columns);
      table.MapColumn("ChildId", "Child", children.ToDictionary(x => x.Id));
      mapped = mapper.Map<OneToManyParentType>(table, rows).ToArray();
    };

    It should_return_an_instance_for_each_row = () =>
      mapped.Count().ShouldEqual(2);

    It should_set_first_instances_name = () =>
      mapped[0].Name.ShouldEqual("A");

    It should_set_second_instances_name = () =>
      mapped[1].Name.ShouldEqual("C");

    It should_set_first_instances_children = () =>
      mapped[0].Children.ShouldContainOnly(children[0], children[1]);

    It should_set_second_instances_children = () =>
      mapped[1].Children.ShouldContainOnly(children[0], children[1], children[2]);
  }

  public class ChildType
  {
    readonly Int32 _id;
    readonly string _name;

    public Int32 Id
    {
      get { return _id; }
    }

    public string Name
    {
      get { return _name; }
    }

    public ChildType(Int32 id, string name)
    {
      _id = id;
      _name = name;
    }
  }

  public class ParentType
  {
    readonly string _name;
    readonly ChildType _child;

    public string Name
    {
      get { return _name; }
    }

    public ChildType Child
    {
      get { return _child; }
    }

    public ParentType(string name, ChildType child)
    {
      _name = name;
      _child = child;
    }
  }

  public class OneToManyParentType
  {
    readonly string _name;
    readonly ChildType[] _children;

    public string Name
    {
      get { return _name; }
    }

    public ChildType[] Children
    {
      get { return _children; }
    }

    public OneToManyParentType(string name, ChildType[] child)
    {
      _name = name;
      _children = child;
    }
  }
}
