using System;
namespace BattleSimulatorTest
{
  public class Grid
  {
    public int Columns { get; }
    public int Rows { get; }
    public Guid[,] grid;

    public Grid(int columns, int rows)
    {
      Columns = columns;
      Rows = rows;
      grid = new Guid[columns, rows];
    }

    public void PlaceUnit(int column, int row, Guid guid)
    {
      grid[column, row] = guid;
    }

    public Guid GetPosition(int column, int row)
    {
      return grid[column, row];
    }

    public void GetObjectsInRange(int column, int row, int range)
    {
      int columnSearchStart = column - range < 0 ? 0 : column - range;
      int rowSearchStart = row - range < 0 ? 0 : row - range;
      int columnSearchEnd = column + range > Columns ? Columns : column + range;
      int rowSearchEnd = row + range > Rows ? Rows : row + range;
      for (int c = columnSearchStart; c < columnSearchEnd; c++)
        for (int r = rowSearchStart; r < rowSearchEnd; r++)
        {
          if (GetPosition(c, r) != Guid.Empty)
            Console.WriteLine($"Found Object - [{c},{r}]");
        }
    }

  }
}

