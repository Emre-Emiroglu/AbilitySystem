using System.IO;
using UnityEditor;
using UnityEngine;

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
        public static void CreateSoFromMenu(string abilityName)
        {
            string className = $"{abilityName}Data";
            string targetPath = $"{AbilityDataFolder}/{className}.asset";
            
            if (File.Exists(targetPath))
                return;
            
            string menuPath = $"AbilitySystem/AbilityData/{abilityName}Data";

            EditorApplication.ExecuteMenuItem($"Assets/Create/{menuPath}");
            
            EditorApplication.delayCall += () =>
            {
                Object[] selectedObjects = Selection.objects;

                if (selectedObjects.Length <= 0)
                    return;
                
                string sourcePath = AssetDatabase.GetAssetPath(selectedObjects[0]);

                if (string.IsNullOrEmpty(sourcePath) || !sourcePath.EndsWith($"{className}.asset"))
                    return;
                
                string moveResult = AssetDatabase.MoveAsset(sourcePath, targetPath);

                if (!string.IsNullOrEmpty(moveResult))
                    return;
                
                AssetDatabase.SaveAssets();
                            
                AssetDatabase.Refresh();
                            
                Object movedAsset = AssetDatabase.LoadAssetAtPath<Object>(targetPath);
                            
                Selection.activeObject = movedAsset;
                
                EditorGUIUtility.PingObject(movedAsset);
            };

        }
        #endregion        
    }
}
