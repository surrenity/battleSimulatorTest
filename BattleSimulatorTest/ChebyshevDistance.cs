using System;
namespace BattleSimulatorTest
{
  public static class ChebyshevDistance
  {
    public static int Calculate(int startX, int targetX, int startY, int targetY)
    {
      return Math.Max(Math.Abs(targetX - startX), Math.Abs(targetY - startY));
    }
  }
}

