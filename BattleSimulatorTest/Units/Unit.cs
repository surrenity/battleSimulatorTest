using System;
namespace BattleSimulatorTest
{
  public class Unit
  {
    public Guid UnitId { get; set; }
    public string? Name { get; set; }
    public int Health { get; set; }
    public int Initiative { get; set; }
    public int MovementPoints { get; set; }
    public int ManaPoints { get; set; }
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public int AttackRange { get; set; }
    public bool IsAlive
    {
      get {
      return Health > 0;
      }
    }

    public Unit()
    {
      UnitId = Guid.NewGuid();
    }

    public void Log()
    {
      Console.WriteLine($"{Name} - {UnitId} acting with Initiative {Initiative}");
    }

  }
}

