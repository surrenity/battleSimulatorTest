using BattleSimulatorTest;
using System.Linq;
// See https://aka.ms/new-console-template for more information

//New Simulation
//Generate Grid
//Generate Units, Load List of Units
//Place Units on the Grid


//Create Game Board
Grid grid = new Grid(columns: 10, rows: 10);

//Generate Units, Load List of Units
Dictionary<Guid, Unit> units = new Dictionary<Guid, Unit>();

for(int i = 0; i < grid.Columns; i++)
{
  var footie = new Footman($"Chuck{i}");
  footie.TeamId = 1;
  units.Add(footie.UnitId, footie);
}

for (int i = 0; i < grid.Columns; i++)
{
  var footie = new Footman($"Larry{i}");
  footie.TeamId = 2;
  units.Add(footie.UnitId, footie);
}


//Place Soldiers on Grid
var team1Units = units.Where(x => x.Value.TeamId == 1)
  .Select(x => x.Value)
  .ToList();
for (int i = 0; i < team1Units.Count; i++)
{
  grid.PlaceUnit(row: 0, column: i, team1Units[i].UnitId);
}

var team2Units = units.Where(x => x.Value.TeamId == 2)
  .Select(x => x.Value)
  .ToList();
for (int i = 0; i < team2Units.Count; i++)
{
  grid.PlaceUnit(row: 9, column: i, team2Units[i].UnitId);
}


int round = 0;
while (team1Units.Sum(x => x.Health) > 0 && team2Units.Sum(x => x.Health) > 0)
{
  Console.WriteLine($"Round({round}) Team 1 Health: {team1Units.Sum(x => x.Health)} Team 2 Health: {team2Units.Sum(x => x.Health)}");

  grid.PrintSelf(units);

  //Begin Turns in a loop until winner determined
  //Determine Initiative, Resolve ties
  var aliveUnits = units.Where(x => x.Value.IsAlive).Select(x => x.Value);
  foreach (var unit in aliveUnits)
  {
    unit.Initiative = RandomNumber.Generate(0, int.MaxValue);
  }

  //Tie resolution
  var reRollUnits = aliveUnits.GroupBy(x => x.Initiative)
    .Where(x => x.Count() > 1)
    .SelectMany(x => x);
  while (reRollUnits.Count() > 0)
  {
    foreach (var unit in reRollUnits.Select(x => x))
    {
      unit.Initiative = RandomNumber.Generate(0, int.MaxValue);
    }
    reRollUnits = reRollUnits = aliveUnits.GroupBy(x => x.Initiative)
      .Where(x => x.Count() > 1)
      .SelectMany(x => x);
  }

  //Sort units by initiative
  var iniativeSortedUnits = aliveUnits.OrderBy(x => x.Initiative);
  foreach (var unit in iniativeSortedUnits)
  {
    unit.Log();

    //If the unit was meant to act, but it died, we need to skip it's turn.
    if (!unit.IsAlive)
      continue;

    //Units will now findTargets
    //Then they will Attack
    //If they can't Attack, they will move then attak
    //If they can't move and attack, they will just move
    //If they can't move they will skip

    var unitPosition = grid.UnitPositions.Single(x => x.UnitId == unit.UnitId);
    //Search for Objects in attack Range
    var attackTarget = grid.FindTargetWithinAttackRange(row: unitPosition.Row, column: unitPosition.Column, attackRange: unit.AttackRange, new RandomTargetDecision(), units);
    if (attackTarget.IsValid())
    {
      Console.WriteLine($"Unit [{unitPosition.Row},{unitPosition.Column}] will attack TARGET [{attackTarget.Row},{attackTarget.Column}]{attackTarget.UnitId}");
      //Attack and move on
      var dmg = RandomNumber.Generate(unit.MinDamage, unit.MaxDamage);

      if (units.TryGetValue(attackTarget.UnitId, out Unit targetUnit))
      {
        Console.WriteLine($"Unit [{unitPosition.Row},{unitPosition.Column}] {unit.Name} Attacks TARGET [{attackTarget.Row},{attackTarget.Column}]{targetUnit.Name} for {dmg} damage");
        targetUnit.TakeDamage(dmg);
        if (targetUnit.Health <= 0)
        {
          Console.WriteLine($"Unit {targetUnit.Name} has been killed");
          grid.RemoveUnit(attackTarget.Row, attackTarget.Column, targetUnit.UnitId);
        }
      }
      
    }
    else
    {
      Console.WriteLine($"Unit [{unitPosition.Row},{unitPosition.Column}] Has No TARGET within Range");

      //If no units in range, need to find Unit we want to target for movement
      var movementTarget = grid.FindTargetUnitForMovement(row: unitPosition.Row, column: unitPosition.Column, unit.AttackRange, new RandomTargetDecision(), units);
      if (movementTarget.IsValid())
        Console.WriteLine($"Position [{unitPosition.Row},{unitPosition.Column}] is {movementTarget.ChebyshevDistance} move away from the TARGET [{movementTarget.Row},{movementTarget.Column}]{movementTarget.UnitId}");
      else
        Console.WriteLine($"Unit [{unitPosition.Row},{unitPosition.Column}] Has No TARGET");


      //If we have to move to target, this gives us the path
      var movementPath = grid.FindPathToTarget(rowStart: unitPosition.Row, columnStart: unitPosition.Column, movementTarget, attackRange: unit.AttackRange);

      Tile? destinationTile = null;
      for (int i = unit.MovementPoints; i >= 0; i--)
      {
        Tile? currentTile;
        movementPath.TryPop(out currentTile);

        if (currentTile != null && grid.GetPosition(currentTile.X, currentTile.Y) == Guid.Empty)
        {
          Console.WriteLine($"Position[{currentTile.X},{currentTile.Y}] is available for movement");
          destinationTile = currentTile;
        }
      }

      if (destinationTile != null)
      {
        Console.WriteLine($"Position[{destinationTile.X},{destinationTile.Y}] is the destination Tile");
        grid.RemoveUnit(unitPosition.Row, unitPosition.Column, unit.UnitId);
        grid.PlaceUnit(destinationTile.X, destinationTile.Y, unit.UnitId);

      }
      else
        Console.WriteLine($"{unit.UnitId} Can't move this round");
    }

  }

  grid.PrintSelf(units);
  round++;
}

Console.WriteLine($"Team 1 Health: {team1Units.Sum(x => x.Health)} Team 2 Health: {team2Units.Sum(x => x.Health)}");
Console.WriteLine($"Simulation is Over");
grid.PrintSelf(units);








