using AbilitySystem.Runtime.Data;

namespace AbilitySystem.Runtime.Interfaces
{
    public interface IAbilityLogic<in TAbilityData> where TAbilityData : AbilityData
    {
        public void Initialize(TAbilityData abilityData);
        public void Execute();
        public void Cancel();
    }
}