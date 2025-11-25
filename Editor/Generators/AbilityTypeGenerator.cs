using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Runtime.Generators
{
    public static class AbilityTypeGenerator
    {
        #region Constants
        private const string AbilityTypeFolder = "Assets/AbilitySystem2/Scripts/Runtime/Enums";
        private const string AbilityTypeFilePath = "Assets/AbilitySystem2/Scripts/Runtime/Enums/AbilityType.cs";
        private const string Indent = "    "; 
        #endregion

        #region Executes
        public static bool AddAbilityType(string abilityName)
        {
            if (!Directory.Exists(AbilityTypeFolder))
                Directory.CreateDirectory(AbilityTypeFolder);
            
            if (!File.Exists(AbilityTypeFilePath))
            {
                Debug.Log($"AbilityType.cs file not found at: {AbilityTypeFilePath}. Creating a new one.");
                
                string content =
$@"namespace AbilitySystem2.Scripts.Runtime.Enums
{{
{Indent}public enum AbilityType
{Indent}{{
{Indent}}}
}}";
                
                File.WriteAllText(AbilityTypeFilePath, content);
                
                AssetDatabase.Refresh();
            }

            string readContent = File.ReadAllText(AbilityTypeFilePath);

            if (readContent.IndexOf(abilityName + ",", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log($"Element '{abilityName}' already exists in AbilityType enum. No changes were made.");
                
                return false;
            }

            string searchPattern = $"{Indent}}}"; 
            
            int enumClosingIndex = readContent.LastIndexOf(searchPattern, StringComparison.Ordinal);

            if (enumClosingIndex == -1)
                return false;
            
            string contentUntilEnumEnd = readContent[..enumClosingIndex];

            string newEnumEntry = $"{Indent}{Indent}{abilityName},{Environment.NewLine}";

            string newContent = contentUntilEnumEnd +
                                newEnumEntry +
                                searchPattern +
                                readContent[(enumClosingIndex + searchPattern.Length)..];

            File.WriteAllText(AbilityTypeFilePath, newContent);
                
            AssetDatabase.Refresh();
                
            Debug.Log($"'{abilityName}' successfully added to the AbilityType enum.");
            
            return true;
        }
        #endregion
    }
}