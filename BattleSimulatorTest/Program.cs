using BattleSimulatorTest;
using System.Linq;
// See https://aka.ms/new-console-template for more information

int teamSize = 100;
List<Unit> team1Units = new List<Unit>();
List<Unit> team2Units = new List<Unit>();

for (int i = 0; i < teamSize; i++)
{
  var footie = new Footman($"Chuck{i}");
  footie.TeamId = 1;
  team1Units.Add(footie);
}

for (int i = 0; i < teamSize; i++)
{
  var footie = new Footman($"Larry{i}");
  footie.TeamId = 2;
  team2Units.Add(footie);
}

Simulation sim = new Simulation(new SimulationOptions
{
  Columns = 100,
  Rows = 20,
  Team1Units = team1Units,
  Team2Units = team2Units
});

var res = sim.Run();

Console.WriteLine($"Total Time {res.RunTime}");
Console.WriteLine($"Team 1 Health: {res.Team1Units.Sum(x => x.Health)} Team 2 Health: {res.Team2Units.Sum(x => x.Health)}");
Console.WriteLine($"Team 1 Units: {res.Team1Units.Count()} Team 2 Units: {res.Team2Units.Count()}");
Console.WriteLine($"Simulation is Over");




