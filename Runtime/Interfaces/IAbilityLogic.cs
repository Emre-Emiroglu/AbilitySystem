using AbilitySystem.Runtime.Data;

namespace AbilitySystem.Runtime.Interfaces
{
    public interface IAbilityLogic
    {
        public void Initialize(AbilityData abilityData);
        public void Execute();
        public void Cancel();
    }
}