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
        #region Constants
        private const string ResourcesAbilityDataFolder = "AbilitySystem/ScriptableObjects";
        #endregion
        
        #region StaticReadonlyFields
        private static readonly Dictionary<string, AbilityData> DataMap = new();
        private static readonly Dictionary<string, object> AbilityInstanceMap = new();
        #endregion

        #region Executes
        public static void InitializeManager()
        {
            Debug.Log("<color=yellow>AbilityManager: Initializing...</color>");

            LoadAllAbilityData();
            
            CreateAbilityInstances();

            Debug.Log($"<color=green>AbilityManager: {AbilityInstanceMap.Count} abilities ready.</color>");
        }
        private static void LoadAllAbilityData()
        {
            DataMap.Clear();
            
            AbilityData[] allData = Resources.LoadAll<AbilityData>(ResourcesAbilityDataFolder);

            foreach (AbilityData data in allData)
                DataMap.TryAdd(data.AbilityName, data);
        }
        private static void CreateAbilityInstances()
        {
            AbilityInstanceMap.Clear();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> abilityTypes = assemblies.SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            }).Where(t =>
                t.IsClass && !t.IsAbstract && t.BaseType is { IsGenericType: true } &&
                t.BaseType.GetGenericTypeDefinition() == typeof(BaseAbility<>)).ToList();

            foreach (Type type in abilityTypes)
            {
                string abilityName = type.Name.Replace("Ability", "");

                if (!DataMap.TryGetValue(abilityName, out AbilityData abilityData))
                {
                    Debug.LogWarning($"No AbilityData found for ability: {abilityName}");
                    
                    continue;
                }

                object instance = Activator.CreateInstance(type);
                
                MethodInfo initMethod = type.GetMethod("Initialize");

                initMethod?.Invoke(instance, new object[] { abilityData });

                AbilityInstanceMap.Add(abilityName, instance);
            }
        }
        public static void Initialize(string abilityName)
        {
            if (!TryGetInstance(abilityName, out object instance))
                return;

            AbilityData data = DataMap[abilityName];
            
            MethodInfo method = instance.GetType().GetMethod("Initialize");

            method?.Invoke(instance, new object[] { data });
        }
        public static void Execute(string abilityName)
        {
            if (!TryGetInstance(abilityName, out object instance))
                return;

            MethodInfo method = instance.GetType().GetMethod("Execute");

            method?.Invoke(instance, null);
        }
        public static void Cancel(string abilityName)
        {
            if (!TryGetInstance(abilityName, out object instance))
                return;

            MethodInfo method = instance.GetType().GetMethod("Cancel");

            method?.Invoke(instance, null);
        }
        private static bool TryGetInstance(string abilityName, out object instance)
        {
            if (AbilityInstanceMap.TryGetValue(abilityName, out instance))
                return true;
            
            Debug.LogError($"Ability '{abilityName}' is not registered or not found!");
            
            return false;
        }
        #endregion
    }
}