using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Data;
using UnityEngine;

namespace AbilitySystem.Runtime.Managers
{
    public static class AbilityManager
    {
        #region ReadonlyFields
        private static readonly Dictionary<AbilityData, BaseAbility> AbilityInstanceMap = new();
        private static readonly Dictionary<string, AbilityData> DataMap = new();
        #endregion

        #region Executes
        public static void InitializeManager()
        {
            Debug.Log("AbilityManager: Starting initialization...");
            
            List<Type> abilityTypes = CheckAndGetAbilityTypes();

            AbilityData[] allAbilityData = LoadAllAbilityData();

            CheckForRegisterAbilityInstance(abilityTypes, allAbilityData);
            
            Debug.Log($"AbilityManager: Initialization complete. {AbilityInstanceMap.Count} abilities registered.");
        }
        private static List<Type> CheckAndGetAbilityTypes()
        {
            List<Type> abilityTypes = new List<Type>();

            Assembly[] domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in domainAssemblies)
            {
                try
                {
                    if (assembly.FullName.StartsWith("UnityEditor"))
                        continue; 

                    List<Type> typesInAssembly = assembly.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseAbility)))
                        .ToList();
            
                    abilityTypes.AddRange(typesInAssembly);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogWarning(
                        $"AbilityManager: Failed to load types from assembly {assembly.FullName}. Error: {ex.Message}");
                }
            }

            return abilityTypes;
        }
        private static AbilityData[] LoadAllAbilityData() =>
            Resources.LoadAll<AbilityData>(nameof(AbilitySystem) + "/ScriptableObjects");
        private static void CheckForRegisterAbilityInstance(List<Type> abilityTypes, AbilityData[] allAbilityData)
        {
            foreach (Type abilityType in abilityTypes)
            {
                string abilityName = abilityType.Name.Replace("Ability", "");
                
                AbilityData data = allAbilityData.FirstOrDefault(d => d.AbilityName == abilityName);

                if (data)
                {
                    DataMap[abilityName] = data;

                    if (AbilityInstanceMap.ContainsKey(data))
                        return;

                    if (Activator.CreateInstance(abilityType) is BaseAbility abilityInstance)
                    {
                        abilityInstance.Initialize(data);
                
                        AbilityInstanceMap.Add(data, abilityInstance);
                    }
                    else
                        Debug.LogError($"AbilityManager: Failed to create instance of type {abilityType.Name}.");
                }
                else
                    Debug.LogWarning(
                        $"AbilityManager: No matching AbilityData asset found for class {abilityType.Name}.");
            }
        }
        public static void Execute(string abilityName)
        {
            (AbilityData data, BaseAbility ability) abilityTuple = GetAbilityTuple(abilityName);
            
            abilityTuple.ability?.Execute();
        }
        public static void Cancel(string abilityName)
        {
            (AbilityData data, BaseAbility ability) abilityTuple = GetAbilityTuple(abilityName);
            
            abilityTuple.ability?.Cancel();
        }
        private static (AbilityData data, BaseAbility ability) GetAbilityTuple(string abilityName)
        {
            if (DataMap.TryGetValue(abilityName, out AbilityData abilityData))
            {
                if (AbilityInstanceMap.TryGetValue(abilityData, out BaseAbility abilityInstance))
                    return (abilityData, abilityInstance);
            }
            
            Debug.LogError($"AbilityManager: Ability data or instance for {abilityName} not registered or found!");
                
            return (null, null);
        }
        #endregion
    }
}