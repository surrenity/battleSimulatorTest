﻿using System;
namespace BattleSimulatorTest
{
  public class SimulationResult
  {
    public TimeSpan RunTime { get; set; }
    public List<Unit> Team1Units { get; set; }
    public List<Unit> Team2Units { get; set; }

    public SimulationResult()
    {
      Team1Units = new List<Unit>();
      Team2Units = new List<Unit>();
    }
  }
}

