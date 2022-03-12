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

    public void FindUnitsWithinAttackRange(int row, int column, int attackRange)
    {
      int columnSearchStart = column - attackRange < 0 ? 0 : column - attackRange;
      int rowSearchStart = row - attackRange < 0 ? 0 : row - attackRange;
      int columnSearchEnd = column + attackRange > Columns ? Columns : column + attackRange;
      int rowSearchEnd = row + attackRange > Rows ? Rows : row + attackRange;
      for (int r = rowSearchStart; r < rowSearchEnd; r++)
        for (int c = columnSearchStart; c < columnSearchEnd; c++)
        {
          if (GetPosition(c, r) != Guid.Empty)
            Console.WriteLine($"Found Object - [{c},{r}]");
        }
    }

    public void FindTargetUnitForMovement(int rowStart, int columnStart, int attackRange, ITargetDecider targetDecider)
    {
      List<PotentialTarget> potentialTargets = new List<PotentialTarget>();
      int currentMinimumDistance = int.MaxValue;
      foreach(var unitPosition in UnitPositions)
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
      potentialTargets = potentialTargets.Where(x => x.ChebyshevDistance <= currentMinimumDistance).ToList();

      //Decide on the target
      foreach(var potentialTarget in potentialTargets)
      {
        Console.WriteLine($"Position [{rowStart},{columnStart}] is {potentialTarget.ChebyshevDistance} move away with attack range({attackRange}) of [{potentialTarget.Row},{potentialTarget.Column}]{potentialTarget.UnitId}");
      }

      var target = targetDecider.DecideTarget(potentialTargets);

      var fakeTarget = new PotentialTarget
      {
        Row = 3,
        Column = 3,
        ColumnDifference = Math.Abs(columnStart - 3),
        RowDifference = Math.Abs(rowStart - 2),
        ChebyshevDistance =  ChebyshevDistance.Calculate(startX: rowStart, targetX: 2, startY: columnStart,targetY: 3) - attackRange
      };

      target = fakeTarget;

      Console.WriteLine($"Position [{rowStart},{columnStart}] is {target.ChebyshevDistance} move away from the TARGET [{target.Row},{target.Column}]{target.UnitId}");

      AStarSearch search = new AStarSearch();

      var start = new Tile
      {
        Y = columnStart,
        X = rowStart,
      };
      var end = new Tile
      {
        Y = target.Column,
        X = target.Row
      };
      search.SearchWithRange(this, start, end, attackRange);

      //Find shortest Path to target
      //What to do if no Path to target? Can Unit "Move Up?"



    }
  }
}

