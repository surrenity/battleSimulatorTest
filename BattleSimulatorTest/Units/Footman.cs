using System;
namespace BattleSimulatorTest
{
  public class Footman : Unit
  {
    public Footman(string name)
      :base()
    {
      this.Health = 10;
      this.ManaPoints = 0;
      this.MinDamage = 3;
      this.MaxDamage = 7;
      this.MovementPoints = 2;
      this.Name = name;
      this.AttackRange = 1;
    }
  }
}

