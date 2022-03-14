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
      var targetIndex = RandomNumber.Generate(0, potentialTargets.Count);
      return potentialTargets[targetIndex];
    }
  }
}

