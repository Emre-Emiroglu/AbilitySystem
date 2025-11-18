using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Runtime.Generators
{
    public static class AbilityTypeGenerator
    {
        #region Constants
        private const string EnumFolder = "Assets/AbilitySystem/Scripts/Runtime/Enums";
        private const string EnumFilePath = "Assets/AbilitySystem/Scripts/Runtime/Enums/AbilityType.cs";
        private const string Indent = "    "; 
        #endregion

        #region Executes
        public static void AddAbilityType(string abilityName)
        {
            if (string.IsNullOrWhiteSpace(abilityName))
            {
                Debug.LogError("Ability name cannot be empty!");
                
                return;
            }
            
            string sanitizedAbilityName = abilityName.Trim().Replace(" ", string.Empty);
            
            if (!Directory.Exists(EnumFolder))
                Directory.CreateDirectory(EnumFolder);
            
            if (!File.Exists(EnumFilePath))
            {
                Debug.LogWarning($"AbilityType.cs file not found at: {EnumFilePath}. Creating a new one.");
                
                string content = GetContent();
                
                File.WriteAllText(EnumFilePath, content);
                
                AssetDatabase.Refresh();
            }

            string readContent = File.ReadAllText(EnumFilePath);

            if (readContent.IndexOf(sanitizedAbilityName + ",", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.LogWarning(
                    $"Element '{sanitizedAbilityName}' already exists in AbilityType enum. No changes were made.");
                
                return;
            }

            string searchPattern = $"{Indent}}}"; 
            
            int enumClosingIndex = readContent.LastIndexOf(searchPattern, StringComparison.Ordinal);

            if (enumClosingIndex == -1)
                return;
            
            string contentUntilEnumEnd = readContent[..enumClosingIndex];

            string newEnumEntry = $"{Indent}{Indent}{sanitizedAbilityName},{Environment.NewLine}";

            string newContent = contentUntilEnumEnd +
                                newEnumEntry +
                                searchPattern +
                                readContent[(enumClosingIndex + searchPattern.Length)..];

            File.WriteAllText(EnumFilePath, newContent);
                
            AssetDatabase.Refresh();
                
            Debug.Log($"'{sanitizedAbilityName}' successfully added to the AbilityType enum.");
        }
        private static string GetContent()
        {
            string content =
$@"namespace AbilitySystem.Scripts.Runtime.Enums
{{
{Indent}public enum AbilityType
{Indent}{{
{Indent}}}
}}";

            return content;
        }
        #endregion
    }
}