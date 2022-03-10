using BattleSimulatorTest;
using System.Linq;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

//Load Units
Dictionary<Guid, Unit> units = new Dictionary<Guid, Unit>();
var footie = new Footman("Chuck");
units.Add(footie.UnitId, footie);

footie = new Footman("Larry");
units.Add(footie.UnitId, footie);

footie = new Footman("Gary");
units.Add(footie.UnitId, footie);

footie = new Footman("Will");
units.Add(footie.UnitId, footie);

footie = new Footman("Carl");
units.Add(footie.UnitId, footie);

//Create Game Board
Grid grid = new Grid(columns: 10, rows: 10);

//Place Soldiers on Grid
var keys = units.Keys.ToArray();
for (int i = 0; i < units.Count; i++)
{
  grid.PlaceUnit(i, 0, units.GetValueOrDefault(keys[i]).UnitId);
}



//Begin Turns in a loop until winner determined
//Determine Initiative, Resolve ties
var aliveUnits = units.Where(x => x.Value.IsAlive).Select(x => x.Value);
Random random = new Random();
foreach(var unit in aliveUnits)
{
  unit.Initiative = random.Next(0, int.MaxValue);
}

//Add tie resolver
var iniativeSortedUnits = aliveUnits.OrderBy(x => x.Initiative);
foreach(var unit in iniativeSortedUnits)
{
  
  unit.Act();
}

grid.GetObjectsInRange(1, 1, 1);


//Print board
for (int c = 0; c < grid.Columns; c++)
  for (int r = 0; r < grid.Columns; r++)
  {
    var unitId = grid.GetPosition(c, r);
    var initiative = 0;
    if (unitId != Guid.Empty)
      initiative = units[unitId].Initiative;
    Console.Write($"[{c},{r}] - {grid.GetPosition(c, r).ToString()} - initiative: {initiative}");
    Console.WriteLine(Environment.NewLine);
  }








