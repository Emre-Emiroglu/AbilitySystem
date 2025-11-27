using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AbilitySystem.Runtime.Data;
using UnityEngine;

namespace AbilitySystem.Runtime.Managers
{
    public static class AbilityManager
    {
        #region Constants
        private const string ResourcesAbilityDataFolder = "AbilitySystem";
        #endregion
        
        #region Data Maps
        private static readonly Dictionary<string, AbilityData> DataMap = new();
        private static readonly Dictionary<string, Stack<object>> DeActiveAbilityInstanceMap = new();
        private static readonly Dictionary<string, HashSet<object>> ActiveAbilityInstanceMap = new();
        #endregion
        
        #region Initialization
        public static void InitializeManager()
        {
            Debug.Log("<color=yellow>[AbilityManager] Initializing...</color>");

            LoadAllAbilityData();
            
            PrepareDictionaries();

            Debug.Log("<color=green>[AbilityManager] Ready.</color>");
        }
        private static void LoadAllAbilityData()
        {
            DataMap.Clear();

            AbilityData[] allData = Resources.LoadAll<AbilityData>(ResourcesAbilityDataFolder);

            foreach (var data in allData)
                if (!string.IsNullOrWhiteSpace(data.AbilityName))
                    DataMap[data.AbilityName] = data;

            Debug.Log($"[AbilityManager] Loaded {DataMap.Count} AbilityData assets.");
        }
        private static void PrepareDictionaries()
        {
            DeActiveAbilityInstanceMap.Clear();
            
            ActiveAbilityInstanceMap.Clear();

            foreach (string ability in DataMap.Keys)
            {
                DeActiveAbilityInstanceMap[ability] = new Stack<object>(8);
                
                ActiveAbilityInstanceMap[ability] = new HashSet<object>();
            }
        }
        #endregion

        #region Executes
        public static object Spawn(string abilityName)
        {
            if (!DataMap.TryGetValue(abilityName, out AbilityData data))
            {
                Debug.LogError($"[AbilityManager] No AbilityData found for {abilityName}");
                
                return null;
            }

            object instance;

            if (DeActiveAbilityInstanceMap[abilityName].Count > 0)
                instance = DeActiveAbilityInstanceMap[abilityName].Pop();
            else
            {
                Type type = FindAbilityType(abilityName);
                
                if (type == null)
                {
                    Debug.LogError($"[AbilityManager] No Ability class found for {abilityName}");
                    
                    return null;
                }

                instance = Activator.CreateInstance(type);
            }

            MethodInfo initialize = instance.GetType().GetMethod("Initialize");
            
            initialize?.Invoke(instance, new object[] { data });

            ActiveAbilityInstanceMap[abilityName].Add(instance);

            return instance;
        }
        public static void Release(object instance)
        {
            if (instance == null)
                return;

            string abilityName = ExtractAbilityName(instance.GetType().Name);

            if (!ActiveAbilityInstanceMap.ContainsKey(abilityName))
            {
                Debug.LogWarning($"[AbilityManager] Tried to release unknown ability → {abilityName}");
                
                return;
            }

            MethodInfo cancel = instance.GetType().GetMethod("Cancel");
            
            cancel?.Invoke(instance, null);

            ActiveAbilityInstanceMap[abilityName].Remove(instance);
            
            DeActiveAbilityInstanceMap[abilityName].Push(instance);
        }
        public static void CancelAll(string abilityName)
        {
            if (!ActiveAbilityInstanceMap.TryGetValue(abilityName, out HashSet<object> activeAbilityInstances))
                return;

            foreach (object instance in activeAbilityInstances)
                Release(instance);
        }
        public static void Execute(object instance)
        {
            if (instance == null)
                return;

            MethodInfo execute = instance.GetType().GetMethod("Execute");
            
            execute?.Invoke(instance, null);
        }
        public static void ExecuteAll(string abilityName)
        {
            if (!ActiveAbilityInstanceMap.TryGetValue(abilityName, out HashSet<object> activeAbilityInstances))
                return;

            foreach (object instance in activeAbilityInstances)
                Execute(instance);
        }
        private static Type FindAbilityType(string abilityName)
        {
            string expected = abilityName + "Ability";

            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            }).FirstOrDefault(t => t.Name == expected);
        }
        private static string ExtractAbilityName(string typeName) => typeName.Replace("Ability", "");
        #endregion
    }
}
