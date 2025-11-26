using AbilitySystem.Runtime.Data;
using AbilitySystem.Runtime.Interfaces;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public abstract class BaseAbility<TAbilityData> : IAbilityLogic<TAbilityData> where TAbilityData : AbilityData
    {
        #region Fields
        protected TAbilityData AbilityData;
        #endregion

        #region Core
        public virtual void Initialize(TAbilityData abilityData)
        {
            AbilityData = abilityData;
            
            LogMessage($"Ability: {AbilityData.AbilityName} Initialized");
        }
        public virtual void Execute() => LogMessage($"Ability: {AbilityData.AbilityName} Executed");
        public virtual void Cancel() => LogMessage($"Ability: {AbilityData.AbilityName} Canceled");
        #endregion

        #region Executes
        private static void LogMessage(string message) => Debug.Log(message);
        #endregion
    }
}