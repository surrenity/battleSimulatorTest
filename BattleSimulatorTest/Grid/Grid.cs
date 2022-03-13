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

    public PotentialTarget FindTargetWithinAttackRange(int row, int column, int attackRange, ITargetDecider targetDecider)
    {
      List<PotentialTarget> potentialTargets = new List<PotentialTarget>();
      var attackingUnitId = GetPosition(row, column);

      int currentMinimumDistance = int.MaxValue;
      int columnSearchStart = column - attackRange < 0 ? 0 : column - attackRange;
      int rowSearchStart = row - attackRange < 0 ? 0 : row - attackRange;
      int columnSearchEnd = column + attackRange > Columns - 1 ? Columns - 1 : column + attackRange;
      int rowSearchEnd = row + attackRange > Rows - 1 ? Rows - 1 : row + attackRange;
      

      for (int r = rowSearchStart; r <= rowSearchEnd; r++)
      {
        for (int c = columnSearchStart; c <= columnSearchEnd; c++)
        {
          var unitId = GetPosition(r, c);
          if (unitId != Guid.Empty && unitId != attackingUnitId)
          {
            Console.WriteLine($"Found Potential Target Already in Attack Range - [{r},{c}]");

            var unitPosition = UnitPositions.Single(x => x.UnitId == unitId);
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

    public void PrintSelf()
    {
      for (int r = 0; r < Columns; r++)
      {
        Console.Write(r);
        for (int c = 0; c < Columns; c++)
        {
          var unitId = GetPosition(r, c);
          if (unitId != Guid.Empty)
            Console.Write("X");
          else
            Console.Write("-");

        }
        Console.WriteLine(Environment.NewLine);
      }
    }

    public PotentialTarget FindTargetUnitForMovement(int rowStart, int columnStart, int attackRange, ITargetDecider targetDecider)
    {
      List<PotentialTarget> potentialTargets = new List<PotentialTarget>();
      int currentMinimumDistance = int.MaxValue;
      var potentialPositions = UnitPositions.Where(x => !(x.Row == rowStart && x.Column == columnStart));

      foreach (var unitPosition in potentialPositions)
      {
        int columnDifference = Math.Abs(columnStart - unitPosition.Column);
        int rowDifference = Math.Abs(rowStart - unitPosition.Row);
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
        Console.WriteLine($"Position [{rowStart},{columnStart}] is {potentialTarget.ChebyshevDistance} move away with attack range({attackRange}) of [{potentialTarget.Row},{potentialTarget.Column}]{potentialTarget.UnitId}");
      }
      

      //Decide on the target
      var target = targetDecider.DecideTarget(potentialTargets);

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
  }
}

