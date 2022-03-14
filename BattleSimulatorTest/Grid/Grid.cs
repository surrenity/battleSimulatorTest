using System;
namespace BattleSimulatorTest
{
  public class Grid
  {
    public int Columns { get; }
    public int Rows { get; }
    public Guid[,] grid;
    public List<UnitPosition> UnitPositions { get; private set; }

    public Grid(int rows, int columns)
    {
      Columns = columns;
      Rows = rows;
      grid = new Guid[rows, columns];
      UnitPositions = new List<UnitPosition>();
    }

    public void PlaceUnit(int row, int column, Guid guid)
    {
      var unitPosition = new UnitPosition
      {
        Column = column,
        Row = row,
        UnitId = guid
      };

      if (!UnitPositions.Contains(unitPosition))
      {
        UnitPositions.Add(unitPosition);
        grid[row, column] = guid;
      }
    }

    public void RemoveUnit(int row, int column, Guid guid)
    {
      var unit = UnitPositions.SingleOrDefault(x => x.UnitId == guid);
      if(unit != null)
      {
        UnitPositions.Remove(unit);
        grid[row, column] = Guid.Empty;
      }
    }

    public Guid GetPosition(int row, int column)
    {
      return grid[row, column];
    }

    public PotentialTarget FindTargetWithinAttackRange(int row, int column, int attackRange, ITargetDecider targetDecider, Dictionary<Guid, Unit> units)
    {
      List<PotentialTarget> potentialTargets = new List<PotentialTarget>();
      var attackingUnitId = GetPosition(row, column);
      var attackingUnit = units.GetValueOrDefault(attackingUnitId);

      int currentMinimumDistance = int.MaxValue;
      int columnSearchStart = column - attackRange < 0 ? 0 : column - attackRange;
      int rowSearchStart = row - attackRange < 0 ? 0 : row - attackRange;
      int columnSearchEnd = column + attackRange > Columns - 1 ? Columns - 1 : column + attackRange;
      int rowSearchEnd = row + attackRange > Rows - 1 ? Rows - 1 : row + attackRange;

      for (int r = rowSearchStart; r <= rowSearchEnd; r++)
      {
        for (int c = columnSearchStart; c <= columnSearchEnd; c++)
        {
          var targetUnitId = GetPosition(r, c);
          var targetUnit = units.GetValueOrDefault(targetUnitId);
          bool targetUnitIsNotSelf = targetUnitId != attackingUnitId;
          bool targetUnitIsNotBlank = targetUnitId != Guid.Empty;
          bool targetUnitIsOnOpposingTeam = false;
          if (targetUnit != null)
            targetUnitIsOnOpposingTeam = targetUnit.TeamId != attackingUnit.TeamId;

          if (targetUnitIsNotSelf && targetUnitIsNotBlank && targetUnitIsOnOpposingTeam)
          {
            Console.WriteLine($"Found Potential Target Already in Attack Range - [{r},{c}]");

            var unitPosition = UnitPositions.Single(x => x.UnitId == targetUnitId);
            int columnDifference = Math.Abs(c - unitPosition.Column);
            int rowDifference = Math.Abs(r - unitPosition.Row);
            int chebyshevDistance = Math.Max(columnDifference, rowDifference) - attackRange;
            if (chebyshevDistance <= currentMinimumDistance)
            {
              var potentialTarget = new PotentialTarget
              {
                Column = unitPosition.Column,
                Row = unitPosition.Row,
                ColumnDifference = columnDifference,
                RowDifference = rowDifference,
                UnitId = unitPosition.UnitId,
                ChebyshevDistance = chebyshevDistance
              };
              potentialTargets.Add(potentialTarget);
              currentMinimumDistance = chebyshevDistance;
            }
          }
        }
      }

      //Reduce to our closest potential targets.
      potentialTargets = potentialTargets.Where(x => x.ChebyshevDistance <= currentMinimumDistance)
        .ToList();

      //Decide on the target
      var target = new PotentialTarget();
      if (potentialTargets.Any())
        target = targetDecider.DecideTarget(potentialTargets);
      return target;
    }

    public PotentialTarget FindTargetUnitForMovement(int row, int column, int attackRange, ITargetDecider targetDecider, Dictionary<Guid, Unit> units)
    {
      List<PotentialTarget> potentialTargets = new List<PotentialTarget>();
      var attackingUnitId = GetPosition(row, column);
      var attackingUnit = units.GetValueOrDefault(attackingUnitId);
      int currentMinimumDistance = int.MaxValue;
      var potentialPositions = UnitPositions.Where(x => !(x.Row == row && x.Column == column))
        .ToList();
      var sameTeamIds = units.Where(x => x.Value.TeamId == attackingUnit.TeamId)
        .Select(x => x.Value.UnitId)
        .ToList();
      potentialPositions = potentialPositions.ExceptBy(sameTeamIds, x => x.UnitId).ToList();

      foreach (var unitPosition in potentialPositions)
      {
        int columnDifference = Math.Abs(column - unitPosition.Column);
        int rowDifference = Math.Abs(row - unitPosition.Row);
        int chebyshevDistance = Math.Max(columnDifference, rowDifference) - attackRange;
        if (chebyshevDistance <= currentMinimumDistance)
        {
          var potentialTarget = new PotentialTarget
          {
            Column = unitPosition.Column,
            Row = unitPosition.Row,
            ColumnDifference = columnDifference,
            RowDifference = rowDifference,
            UnitId = unitPosition.UnitId,
            ChebyshevDistance = chebyshevDistance
          };
          potentialTargets.Add(potentialTarget);
          currentMinimumDistance = chebyshevDistance;
        }
      }

      //Reduce to our closest potential targets.
      potentialTargets = potentialTargets.Where(x => x.ChebyshevDistance <= currentMinimumDistance)
        .ToList();
      
      foreach(var potentialTarget in potentialTargets)
      {
        Console.WriteLine($"Position [{row},{column}] is {potentialTarget.ChebyshevDistance} move away with attack range({attackRange}) of [{potentialTarget.Row},{potentialTarget.Column}]{potentialTarget.UnitId}");
      }


      //Decide on the target
      var target = new PotentialTarget();
      if (potentialTargets.Any())
        target = targetDecider.DecideTarget(potentialTargets);

      return target;
    }

    public Stack<Tile> FindPathToTarget(int rowStart, int columnStart, PotentialTarget potentialTarget, int attackRange)
    {
      //Find shortest Path to target withing Range
      AStarSearch search = new AStarSearch(new AStarSearchOptions
      {
        IgnoreUnits = false
      });

      var start = new Tile
      {
        Y = columnStart,
        X = rowStart,
      };
      var end = new Tile
      {
        Y = potentialTarget.Column,
        X = potentialTarget.Row
      };

      var pathTiles = search.SearchWithRange(this, start, end, attackRange);
      if (pathTiles.Count() > 0)
      {
        Console.WriteLine($"We are {pathTiles.Count()} away from the target");
      }
      else
      {
        //What to do if no Path to target? Can Unit "Move Up?"
        Console.WriteLine($"We have no Path to Target");
        search = new AStarSearch(new AStarSearchOptions
        {
          IgnoreUnits = true
        });
        pathTiles = search.SearchForExactPosition(this, start, end);
      }

      return pathTiles;
    }

    public void PrintSelf(Dictionary<Guid, Unit> units)
    {
      for (int r = 0; r < Rows; r++)
      {
        Console.Write(r);
        for (int c = 0; c < Columns; c++)
        {
          var unitId = GetPosition(r, c);
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

