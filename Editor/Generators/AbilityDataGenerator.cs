using System;
using System.IO;
using AbilitySystem.Runtime.Data;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AbilitySystem.Editor.Generators
{
    public static class AbilityDataGenerator
    {
        #region Constants
        private const string AbilityDataFolder = "Assets/Resources/AbilitySystem";
        private const string AbilityDataClassFolder = "Assets/AbilitySystem/Runtime/Data";
        private const string Indent = "    ";
        #endregion

        #region Executes
        public static void CreateAbilityData(string abilityName)
        {
            if (!Directory.Exists(AbilityDataFolder))
                Directory.CreateDirectory(AbilityDataFolder);

            if (!Directory.Exists(AbilityDataClassFolder))
                Directory.CreateDirectory(AbilityDataClassFolder);

            string className = $"{abilityName}Data";
            string classFilePath = $"{AbilityDataClassFolder}/{className}.cs";
            string assetPath = $"{AbilityDataFolder}/{className}.asset";

            if (File.Exists(classFilePath) || File.Exists(assetPath))
                return;

            string content =
$@"using UnityEngine;

namespace AbilitySystem.Runtime.Data
{{
{Indent}[CreateAssetMenu(fileName = ""{className}"", menuName = ""AbilitySystem/AbilityData/{className}"")]
{Indent}public sealed class {className} : AbilityData
{Indent}{{
{Indent}}}
}}";

            File.WriteAllText(classFilePath, content);

            AssetDatabase.Refresh();
        }
        public static void CreateSo(string abilityName)
        {
            string className = $"{abilityName}Data";
            string targetPath = $"{AbilityDataFolder}/{className}.asset";

            string[] guids = AssetDatabase.FindAssets($"{className} t:MonoScript");

            string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            
            MonoScript mono = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);

            Type type = mono.GetClass();

            ScriptableObject instance = ScriptableObject.CreateInstance(type);
            
            AbilityData data = instance as AbilityData;

            if (data)
            {
                data.AbilityName = abilityName;
                
                EditorUtility.SetDirty(data);
            }

            AssetDatabase.CreateAsset(instance, targetPath);

            AssetDatabase.SaveAssets();
            
            AssetDatabase.Refresh();

            Selection.activeObject = instance;
            
            EditorGUIUtility.PingObject(instance);
        }
        #endregion        
    }
}
