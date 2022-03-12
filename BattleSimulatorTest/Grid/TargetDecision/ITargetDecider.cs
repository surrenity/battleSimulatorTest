namespace BattleSimulatorTest
{
  public interface ITargetDecider
  {
    PotentialTarget DecideTarget(List<PotentialTarget> potentialTargets);
  }
}