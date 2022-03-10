using System;
namespace BattleSimulatorTest
{
  public class Archer : Unit
  {
    public Archer(string name)
      :base()
    {
      this.Health = 6;
      this.ManaPoints = 0;
      this.MinDamage = 4;
      this.MaxDamage = 9;
      this.MovementPoints = 1;
      this.Name = name;
      this.AttackRange = 4;
    }
  }
}

