using System;
namespace BattleSimulatorTest
{
  public class Tile
  {
    public int X { get; set; }
    public int Y { get; set; }
    public int Cost { get; set; }
    public int Distance { get; set; }
    public int CostDistance => Cost + Distance;
    public Tile Parent { get; set; }

    public Tile()
    {
    }

    //The distance is essentially the estimated distance, ignoring walls to our target. 
    //So how many tiles left and right, up and down, ignoring walls, to get there. 
    public void CalculateChebyshevDistance(int targetX, int targetY)
    {
      this.Distance = ChebyshevDistance.Calculate(X, targetX, Y, targetY);
    }


  }
}

