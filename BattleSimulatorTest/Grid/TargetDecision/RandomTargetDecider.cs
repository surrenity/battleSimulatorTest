using System;

namespace BattleSimulatorTest
{
  public class RandomTargetDecision : ITargetDecider
  {
    public RandomTargetDecision() 
    {
    }

    public PotentialTarget DecideTarget(List<PotentialTarget> potentialTargets)
    {
      Random random = new Random();
      var targetIndex = random.Next(0, potentialTargets.Count);
      return potentialTargets[targetIndex];
    }
  }
}

