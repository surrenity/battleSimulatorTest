using BattleSimulatorTest;
using System.Linq;
// See https://aka.ms/new-console-template for more information

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
  grid.PlaceUnit(row: 0, column: i, units.GetValueOrDefault(keys[i]).UnitId);
}

footie = new Footman("Sunil");
units.Add(footie.UnitId, footie);
grid.PlaceUnit(row: 9, column: 9, footie.UnitId);


footie = new Footman("Shamma");
units.Add(footie.UnitId, footie);
grid.PlaceUnit(row: 2, column: 2, footie.UnitId);

footie = new Footman("Zamma");
units.Add(footie.UnitId, footie);
grid.PlaceUnit(row: 3, column: 3, footie.UnitId);


footie = new Footman("ZammaBlocker");
units.Add(footie.UnitId, footie);
grid.PlaceUnit(row: 4, column: 4, footie.UnitId);

footie = new Footman("ZammaBlocker");
units.Add(footie.UnitId, footie);
grid.PlaceUnit(row: 4, column: 3, footie.UnitId);

footie = new Footman("ZammaBlocker");
units.Add(footie.UnitId, footie);
grid.PlaceUnit(row: 4, column: 2, footie.UnitId);

footie = new Footman("ZammaBlocker");
units.Add(footie.UnitId, footie);
grid.PlaceUnit(row: 4, column: 1, footie.UnitId);

footie = new Footman("ZammaBlocker");
units.Add(footie.UnitId, footie);
grid.PlaceUnit(row: 4, column: 0, footie.UnitId);

////Place Wall of soldiers on Grid
//for (int i = 0; i < 10; i++)
//{
//  footie = new Footman($"Wall{i}");
//  units.Add(footie.UnitId, footie);
//  grid.PlaceUnit(row: 7, column: i, footie.UnitId);
//}

for (int r = 0; r < grid.Columns; r++)
{
  Console.Write(r);
  for (int c = 0; c < grid.Columns; c++)
  {
    var unitId = grid.GetPosition(r, c);
    if (unitId != Guid.Empty)
      Console.Write("X");
    else
      Console.Write("-");

  }
  Console.WriteLine(Environment.NewLine);
}



//Begin Turns in a loop until winner determined
//Determine Initiative, Resolve ties
var aliveUnits = units.Where(x => x.Value.IsAlive).Select(x => x.Value);
Random random = new Random();
foreach(var unit in aliveUnits)
{
  unit.Initiative = random.Next(0, int.MaxValue);
}

//Tie resolution
var reRollUnits = aliveUnits.GroupBy(x => x.Initiative)
  .Where(x => x.Count() > 1)
  .SelectMany(x => x);
while(reRollUnits.Count() > 0)
{
  foreach (var unit in reRollUnits.Select(x => x))
  {
    unit.Initiative = random.Next(0, int.MaxValue);
  }
  reRollUnits = reRollUnits = aliveUnits.GroupBy(x => x.Initiative)
    .Where(x => x.Count() > 1)
    .SelectMany(x => x);
}

//Sort units by initiative
var iniativeSortedUnits = aliveUnits.OrderBy(x => x.Initiative);
foreach(var unit in iniativeSortedUnits)
{
  unit.Act();
}

//Search for Objects in Range 
grid.FindUnitsWithinAttackRange(row: 1, column: 1, 1);

//If no units in range, need to find Unit we want to target for movement
grid.FindTargetUnitForMovement(rowStart: 9, columnStart: 0, 2, new RandomTargetDecision());




//Print board
for (int r = 0; r < grid.Columns; r++)
{ 
  for (int c = 0; c < grid.Columns; c++)
  {
    var unitId = grid.GetPosition(r, c);
    var initiative = 0;
    if (unitId != Guid.Empty)
      initiative = units[unitId].Initiative;
    Console.Write($"[{r},{c}] - {grid.GetPosition(r, c).ToString()} - initiative: {initiative}");
  }
  Console.WriteLine(Environment.NewLine);
}







