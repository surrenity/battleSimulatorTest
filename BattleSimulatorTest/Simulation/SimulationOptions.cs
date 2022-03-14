using System;
namespace BattleSimulatorTest
{
  public class SimulationOptions
  {
    public int Columns { get; set; }
    public int Rows { get; set; }
    public List<Unit> Team1Units { get; set; }
    public List<Unit> Team2Units { get; set; }

    public SimulationOptions()
    {
      Team1Units = new List<Unit>();
      Team2Units = new List<Unit>();
    }
  }
}

