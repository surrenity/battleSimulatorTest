using System;
using System.Diagnostics.CodeAnalysis;

namespace BattleSimulatorTest
{
  public class PotentialTarget
  {
    public int Column { get; set; }
    public int Row { get; set; }
    public int ColumnDifference { get; set; }
    public int RowDifference { get; set; }
    public Guid UnitId { get; set; }
    public int ChebyshevDistance { get; set; }

    public PotentialTarget()
    {
    }

    public bool IsEmpty()
    {
      return UnitId == Guid.Empty;
    }

    public bool IsValid()
    {
      return UnitId != Guid.Empty;
    }

  }
}

