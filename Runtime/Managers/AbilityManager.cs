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
        private const string ResourcesAbilityDataFolder = "AbilitySystem";
        #endregion
        
        #region StaticReadonlyFields
        private static readonly Dictionary<string, AbilityData> DataMap = new();
        private static readonly Dictionary<string, object> AbilityInstanceMap = new();
        #endregion

        #region Executes
        public static void InitializeManager()
        {
            Debug.Log("<color=yellow>[AbilityManager] Initializing...</color>");

            LoadAllAbilityData();  
            
            CreateAbilityInstances();

            Debug.Log(
                $"<color=green>[AbilityManager] Initialization complete. Total abilities: {AbilityInstanceMap.Count}</color>");
        }
        private static void LoadAllAbilityData()
        {
            Debug.Log("[AbilityManager] Loading AbilityData from Resources...");

            DataMap.Clear();

            AbilityData[] allData = Resources.LoadAll<AbilityData>(ResourcesAbilityDataFolder);

            Debug.Log($"[AbilityManager] Found {allData.Length} AbilityData assets.");

            foreach (AbilityData data in allData)
            {
                if (string.IsNullOrWhiteSpace(data.AbilityName))
                {
                    Debug.LogWarning($"[AbilityManager] AbilityData missing AbilityName → {data.name}");
                    
                    continue;
                }

                if (!DataMap.TryAdd(data.AbilityName, data))
                {
                    Debug.LogWarning($"[AbilityManager] Duplicate AbilityData name detected: {data.AbilityName}");
                    
                    continue;
                }

                Debug.Log($"[AbilityManager] Registered AbilityData → {data.AbilityName}");
            }
        }
        private static void CreateAbilityInstances()
        {
            Debug.Log("[AbilityManager] Searching for all BaseAbility<> implementations...");

            AbilityInstanceMap.Clear();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> abilityTypes = assemblies
                .SelectMany(a =>
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
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.BaseType is { IsGenericType: true } &&
                    t.BaseType.GetGenericTypeDefinition() == typeof(BaseAbility<>)).ToList();

            Debug.Log($"[AbilityManager] Found {abilityTypes.Count} ability classes.");

            foreach (Type type in abilityTypes)
            {
                string abilityName = type.Name.Replace("Ability", "");

                Debug.Log($"[AbilityManager] Processing ability class → {type.Name}");

                if (!DataMap.TryGetValue(abilityName, out AbilityData abilityData))
                {
                    Debug.LogWarning(
                        $"[AbilityManager] No AbilityData found for ability '{abilityName}'. Skipping instance creation.");
                    
                    continue;
                }

                try
                {
                    object instance = Activator.CreateInstance(type);
                    
                    Debug.Log($"[AbilityManager] Created instance → {type.Name}");

                    MethodInfo initMethod = type.GetMethod("Initialize");

                    if (initMethod == null)
                    {
                        Debug.LogError($"[AbilityManager] ERROR: Initialize() method not found in {type.Name}");
                        
                        continue;
                    }

                    initMethod.Invoke(instance, new object[] { abilityData });

                    AbilityInstanceMap.Add(abilityName, instance);

                    Debug.Log($"<color=cyan>[AbilityManager] Registered ability instance → {abilityName}</color>");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AbilityManager] ERROR creating instance of {type.Name}: {ex.Message}");
                }
            }
        }
        public static void Initialize(string abilityName)
        {
            Debug.Log($"[AbilityManager] Initialize() requested → {abilityName}");

            if (!TryGetInstance(abilityName, out object instance))
                return;

            AbilityData data = DataMap[abilityName];

            MethodInfo method = instance.GetType().GetMethod("Initialize");
            
            method?.Invoke(instance, new object[] { data });

            Debug.Log($"<color=cyan>[AbilityManager] Initialize() executed → {abilityName}</color>");
        }
        public static void Execute(string abilityName)
        {
            Debug.Log($"[AbilityManager] Execute() requested → {abilityName}");

            if (!TryGetInstance(abilityName, out object instance))
                return;

            MethodInfo method = instance.GetType().GetMethod("Execute");
            
            method?.Invoke(instance, null);

            Debug.Log($"<color=lime>[AbilityManager] Execute() executed → {abilityName}</color>");
        }
        public static void Cancel(string abilityName)
        {
            Debug.Log($"[AbilityManager] Cancel() requested → {abilityName}");

            if (!TryGetInstance(abilityName, out object instance))
                return;

            MethodInfo method = instance.GetType().GetMethod("Cancel");
            
            method?.Invoke(instance, null);

            Debug.Log($"<color=orange>[AbilityManager] Cancel() executed → {abilityName}</color>");
        }
        private static bool TryGetInstance(string abilityName, out object instance)
        {
            if (AbilityInstanceMap.TryGetValue(abilityName, out instance))
                return true;

            if (!DataMap.ContainsKey(abilityName))
            {
                Debug.LogError($"[AbilityManager] ERROR: AbilityData not found → {abilityName}");
                
                return false;
            }

            Debug.LogError($"[AbilityManager] ERROR: Ability instance not registered → {abilityName}");
            
            return false;
        }
        #endregion
    }
}
