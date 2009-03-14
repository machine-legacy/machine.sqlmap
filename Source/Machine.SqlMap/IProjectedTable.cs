using System;
using System.Collections.Generic;

namespace Machine.SqlMap
{
  public interface IProjectedTable
  {
    Table ToTable();
    IEnumerable<object[]> Rows();
  }
}