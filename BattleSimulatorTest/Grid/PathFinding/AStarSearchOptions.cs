using System;
namespace BattleSimulatorTest
{
  public class AStarSearchOptions
  {
    public bool IgnoreUnits { get; set; }
    public AStarSearchOptions()
    {
      IgnoreUnits = false;
    }
  }
}

