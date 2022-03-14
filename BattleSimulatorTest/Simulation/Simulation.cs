using System;
namespace BattleSimulatorTest
{
  public class Simulation
  {
    public SimulationOptions Options { get; }
    public Grid Grid { get; private set; }
    public List<Unit> Team1Units { get; private set; }
    public List<Unit> Team2Units { get; private set; }
    public Dictionary<Guid, Unit> Units { get; private set; }

    public Simulation(SimulationOptions options)
    {
      Options = options;
      Team1Units = new List<Unit>();
      Team2Units = new List<Unit>();
      Units = new Dictionary<Guid, Unit>();
    }

    public Grid CreateGrid(int rows, int columns)
    {
      return new Grid(rows: 40, columns: 100);
    }

    public SimulationResult Run()
    {
      //Create Game Board
      InitializeBoard();

      //Load Units
      LoadUnits();

      //Place Soldiers on Grid
      PlaceUnitsOnGrid();

      int round = 0;
      DateTime startTime = DateTime.Now;
      while (Team1Units.Any() && Team2Units.Any())
      {
        Console.WriteLine($"Round({round}) Team 1 Health: {Team1Units.Sum(x => x.Health)} Team 2 Health: {Team2Units.Sum(x => x.Health)}");

        PrintGrid(Units);

        //Sort units by initiative
        var iniativeSortedUnits = DetermineAliveUnitInitiatives(Units);

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

          var unitPosition = Grid.UnitPositions.Single(x => x.UnitId == unit.UnitId);
          //Search for Objects in attack Range
          var attackTarget = Grid.FindTargetWithinAttackRange(row: unitPosition.Row, column: unitPosition.Column, attackRange: unit.AttackRange, new RandomTargetDecision(), Units);

          if (attackTarget.IsValid())
          {
            PerformAttack(unit, unitPosition, attackTarget);
          }
          else
          {
            Console.WriteLine($"Unit [{unitPosition.Row},{unitPosition.Column}] Has No TARGET within Range");

            //If no units in range, need to find Unit we want to target for movement
            var movementTarget = Grid.FindTargetUnitForMovement(row: unitPosition.Row, column: unitPosition.Column, unit.AttackRange, new RandomTargetDecision(), Units);
            if (movementTarget.IsValid())
            {
              Console.WriteLine($"Position [{unitPosition.Row},{unitPosition.Column}] is {movementTarget.ChebyshevDistance} move away from the TARGET [{movementTarget.Row},{movementTarget.Column}]{movementTarget.UnitId}");
              MoveToTarget(unit, unitPosition, movementTarget);
            }
            else
              Console.WriteLine($"Unit [{unitPosition.Row},{unitPosition.Column}] Has No TARGET");
            
          }
        }

        PrintGrid(Units);
        round++;
      }


      return new SimulationResult
      {
        RunTime = (DateTime.Now - startTime),
        Team1Units = Team1Units,
        Team2Units = Team2Units
      };
    }

    private void InitializeBoard()
    {
      Grid = CreateGrid(rows: Options.Rows, columns: Options.Columns);
    }

    private void LoadUnits()
    {
      Units = new Dictionary<Guid, Unit>();
      Team1Units = Options.Team1Units.ConvertAll(x => new Unit
      {
        AttackRange = x.AttackRange,
        Health = x.Health,
        ManaPoints = x.ManaPoints,
        MaxDamage = x.MaxDamage,
        MinDamage = x.MinDamage,
        MovementPoints = x.MovementPoints,
        Name = x.Name,
        UnitId = x.UnitId,
        TeamId = x.TeamId

      });
      Team2Units = Options.Team2Units.ConvertAll(x => new Unit
      {
        AttackRange = x.AttackRange,
        Health = x.Health,
        ManaPoints = x.ManaPoints,
        MaxDamage = x.MaxDamage,
        MinDamage = x.MinDamage,
        MovementPoints = x.MovementPoints,
        Name = x.Name,
        UnitId = x.UnitId,
        TeamId = x.TeamId

      });

      //Generate Units, Load List of Units
      foreach (var unit in Team1Units)
        Units.Add(unit.UnitId, unit);
      foreach (var unit in Team2Units)
        Units.Add(unit.UnitId, unit);
    }

    private void MoveToTarget(Unit unit, UnitPosition unitPosition, PotentialTarget movementTarget)
    {
      //If we have to move to target, this gives us the path
      var movementPath = Grid.FindPathToTarget(rowStart: unitPosition.Row, columnStart: unitPosition.Column, movementTarget, attackRange: unit.AttackRange);

      Tile? destinationTile = null;
      for (int i = unit.MovementPoints; i >= 0; i--)
      {
        Tile? currentTile;
        movementPath.TryPop(out currentTile);

        if (currentTile != null && Grid.GetPosition(currentTile.X, currentTile.Y) == Guid.Empty)
        {
          Console.WriteLine($"Position[{currentTile.X},{currentTile.Y}] is available for movement");
          destinationTile = currentTile;
        }
      }

      if (destinationTile != null)
      {
        Console.WriteLine($"Position[{destinationTile.X},{destinationTile.Y}] is the destination Tile");
        Grid.RemoveUnit(unitPosition.Row, unitPosition.Column, unit.UnitId);
        Grid.PlaceUnit(destinationTile.X, destinationTile.Y, unit.UnitId);
      }
      else
        Console.WriteLine($"{unit.UnitId} Can't move this round");
    }

    private void PerformAttack(Unit unit, UnitPosition unitPosition, PotentialTarget attackTarget)
    {
      Console.WriteLine($"Unit [{unitPosition.Row},{unitPosition.Column}] will attack TARGET [{attackTarget.Row},{attackTarget.Column}]{attackTarget.UnitId}");
      //Attack and move on
      var dmg = RandomNumber.Generate(unit.MinDamage, unit.MaxDamage);

      if (Units.TryGetValue(attackTarget.UnitId, out Unit targetUnit))
      {
        Console.WriteLine($"Unit [{unitPosition.Row},{unitPosition.Column}] {unit.Name} Attacks TARGET [{attackTarget.Row},{attackTarget.Column}]{targetUnit.Name} for {dmg} damage");
        targetUnit.TakeDamage(dmg);
        if (targetUnit.Health <= 0)
        {
          Console.WriteLine($"Unit {targetUnit.Name} has been killed");
          Grid.RemoveUnit(attackTarget.Row, attackTarget.Column, targetUnit.UnitId);
          Units.Remove(targetUnit.UnitId);
          if (targetUnit.TeamId == 1)
            Team1Units.Remove(targetUnit);
          if (targetUnit.TeamId == 2)
            Team2Units.Remove(targetUnit);
        }
      }
    }

    private void PlaceUnitsOnGrid()
    {
      for (int i = 0; i < Team1Units.Count; i++)
      {
        Grid.PlaceUnit(row: 0, column: i, Team1Units[i].UnitId);
      }

      for (int i = 0; i < Team2Units.Count; i++)
      {
        Grid.PlaceUnit(row: Grid.Rows - 1, column: i, Team2Units[i].UnitId);
      }
    }

    private IOrderedEnumerable<Unit> DetermineAliveUnitInitiatives(Dictionary<Guid, Unit> units)
    {
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

      return aliveUnits.OrderBy(x => x.Initiative);
    }

    private void PrintGrid(Dictionary<Guid, Unit> units)
    {
      for (int r = 0; r < Grid.Rows; r++)
      {
        Console.Write(r);
        for (int c = 0; c < Grid.Columns; c++)
        {
          var unitId = Grid.GetPosition(r, c);
          units.TryGetValue(unitId, out Unit unit);
          if (unitId != Guid.Empty)
          {
            var letter = unit.TeamId == 1 ? "X" : "O";
            Console.Write(letter);
          }
          else
            Console.Write("-");

        }
        Console.WriteLine(Environment.NewLine);
      }
    }

  }
}

