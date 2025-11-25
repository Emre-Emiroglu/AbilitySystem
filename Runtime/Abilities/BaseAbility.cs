using AbilitySystem.Runtime.Data;
using AbilitySystem.Runtime.Interfaces;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public abstract class BaseAbility : IAbilityLogic
    {
        #region Fields
        private AbilityData _abilityData;
        #endregion

        #region Getters
        public AbilityData AbilityData => _abilityData;
        #endregion
        
        #region Core
        public virtual void Initialize(AbilityData abilityData)
        {
            _abilityData = abilityData;
            
            LogMessage($"Ability: {_abilityData.AbilityName} Initialized");
        }
        public virtual void Execute() => LogMessage($"Ability: {_abilityData.AbilityName} Executed");
        public virtual void Cancel() => LogMessage($"Ability: {_abilityData.AbilityName} Canceled");
        #endregion

        #region Executes
        private static void LogMessage(string message) => Debug.Log(message);
        #endregion
    }
}