using System;
using System.Diagnostics.CodeAnalysis;

namespace BattleSimulatorTest
{
  public class UnitPosition : IEquatable<UnitPosition>
  {
    public int Column { get; set; }
    public int Row { get; set; }
    public Guid UnitId { get; set; }

    public UnitPosition()
    {
    }

    public bool Equals(UnitPosition? other)
    {
      if (other == null)
        return false;

      if (object.ReferenceEquals(this, other))
        return true;

      return this.Column == other.Column && this.Row == other.Row;
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as UnitPosition);
    }

    public override int GetHashCode()
    {
      int tmp = (this.Row + ((this.Column + 1) / 2));
      return this.Column + (tmp * tmp);
    }
  }
}

