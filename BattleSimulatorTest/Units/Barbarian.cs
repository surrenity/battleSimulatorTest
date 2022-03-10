using System;
namespace BattleSimulatorTest
{
  public class Barbarian : Unit
  {
    public Barbarian(string name)
      :base()
    {
      this.Health = 8;
      this.ManaPoints = 0;
      this.MinDamage = 5;
      this.MaxDamage = 10;
      this.MovementPoints = 3;
      this.Name = name;
      this.AttackRange = 1;
    }
  }
}

