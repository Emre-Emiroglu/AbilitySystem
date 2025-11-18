using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Data;
using UnityEngine;

namespace AbilitySystem.Runtime.Managers
{
    public static class AbilityManager
    {
        #region ReadonlyFields
        private static readonly Dictionary<AbilityData, BaseAbility> AbilityMap = new();
        #endregion

        #region Executes
        public static void Register(AbilityData abilityData, BaseAbility ability) =>
            AbilityMap.TryAdd(abilityData, ability);
        public static void Initialize(string abilityName)
        {
            (AbilityData abilityData, BaseAbility ability) abilityTuple = GetAbilityTuple(abilityName);

            abilityTuple.ability?.Initialize(abilityTuple.abilityData);
        }
        public static void Execute(string abilityName)
        {
            (AbilityData abilityData, BaseAbility ability) abilityTuple = GetAbilityTuple(abilityName);

            abilityTuple.ability?.Execute();
        }
        public static void Cancel(string abilityName)
        {
            (AbilityData abilityData, BaseAbility ability) abilityTuple = GetAbilityTuple(abilityName);

            abilityTuple.ability?.Cancel();
        }
        private static (AbilityData abilityData, BaseAbility ability) GetAbilityTuple(string abilityName)
        {
            AbilityData abilityData = AbilityMap.Keys.FirstOrDefault(x => x.AbilityName == abilityName);

            if (abilityData && AbilityMap.TryGetValue(abilityData, out BaseAbility ability))
                return (abilityData, ability);
            
            Debug.LogError($"Ability data for {abilityName} not registered!");
                
            return (null, null);
        }
        #endregion
    }
}