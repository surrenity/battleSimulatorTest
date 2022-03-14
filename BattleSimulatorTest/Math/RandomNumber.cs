using System;
namespace BattleSimulatorTest
{
  public static class RandomNumber
  {
    //Function to get random number
    private static readonly Random random = new Random();
    private static readonly object syncLock = new object();
    public static int Generate(int min, int max)
    {
      lock (syncLock)
      { // synchronize
        return random.Next(min, max);
      }
    }
  }
}

