using System;
namespace BattleSimulatorTest
{

  //https://gist.github.com/DotNetCoreTutorials/08b0210616769e81034f53a6a420a6d9
  public class AStarSearch
  {
    public AStarSearch()
    {

    }

    public void SearchForExactPosition(Grid grid, Tile start, Tile finish)
    {
      this.SearchWithRange(grid, start, finish, 0);
    }

    public void SearchWithRange(Grid grid, Tile start, Tile finish, int attackRange)
    {
      start.CalculateChebyshevDistance(finish.X, finish.Y);

      var activeTiles = new List<Tile>();
      activeTiles.Add(start);
      var visitedTiles = new List<Tile>();

      while (activeTiles.Any())
      {
        var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

        //if (checkTile.X == finish.X && checkTile.Y == finish.Y) //Exact Match
        if (ChebyshevDistance.Calculate(checkTile.X, finish.X, checkTile.Y, finish.Y) <= attackRange) //Match within Range
        {
          //We found the destination and we can be sure (Because the the OrderBy above)
          //That it's the most low cost option. 
          var tile = checkTile;
          Console.WriteLine("Retracing steps backwards...");
          while (true)
          {
            Console.WriteLine($"{tile.X} : {tile.Y}");
            tile = tile.Parent;
            if (tile == null)
            {
              Console.WriteLine("Done!");
              return;
            }
          }
        }

        visitedTiles.Add(checkTile);
        activeTiles.Remove(checkTile);

        var walkableTiles = GetWalkableTiles(grid, checkTile, finish);

        foreach (var walkableTile in walkableTiles)
        {
          //We have already visited this tile so we don't need to do so again!
          if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
            continue;

          //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
          if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
          {
            var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
            if (existingTile.CostDistance > checkTile.CostDistance)
            {
              activeTiles.Remove(existingTile);
              activeTiles.Add(walkableTile);
            }
          }
          else
          {
            //We've never seen this tile before so add it to the list. 
            activeTiles.Add(walkableTile);
          }
        }
      }

      Console.WriteLine("No Path Found!");
    }

    private static List<Tile> GetWalkableTiles(Grid grid, Tile currentTile, Tile targetTile)
    {
      var possibleTiles = new List<Tile>()
            {
                new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
                new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                //Diagonals
                new Tile { X = currentTile.X + 1, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X + 1, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X - 1, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new Tile { X = currentTile.X - 1, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
            };

      possibleTiles.ForEach(tile => tile.CalculateChebyshevDistance(targetTile.X, targetTile.Y));


      return possibleTiles
              .Where(tile => tile.X >= 0 && tile.X <= grid.Rows-1)
              .Where(tile => tile.Y >= 0 && tile.Y <= grid.Columns-1)
              .Where(tile => grid.GetPosition(tile.X, tile.Y) == Guid.Empty || grid.GetPosition(tile.X, tile.Y) == grid.GetPosition(targetTile.X, targetTile.Y) )
              .ToList();
    }

  }

}


