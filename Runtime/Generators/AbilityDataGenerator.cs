using System.IO;
using AbilitySystem.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Runtime.Generators
{
    public static class AbilityDataGenerator
    {
        #region Constants
        private const string AbilityDataFolder = "Assets/Resources/AbilitySystem/ScriptableObjects";
        #endregion

        #region Executes
        public static bool CreateAbilityData(string abilityName)
        {
            if (!Directory.Exists(AbilityDataFolder))
                Directory.CreateDirectory(AbilityDataFolder);
            
            string assetName = $"{abilityName}Data.asset";
            string filePath = $"{AbilityDataFolder}/{assetName}";
            
            if (File.Exists(filePath))
            {
                Debug.LogWarning($"{assetName} already exists!");
                
                return false;
            }
            
            AbilityData data = ScriptableObject.CreateInstance<AbilityData>();
            
            data.AbilityName = abilityName;
            
            AssetDatabase.CreateAsset(data, filePath);
            
            AssetDatabase.SaveAssets();

            return true;
        }
        #endregion
    }
}