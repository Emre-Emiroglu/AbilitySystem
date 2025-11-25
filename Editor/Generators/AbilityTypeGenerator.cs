using System;
using System.IO;
using UnityEditor;

namespace AbilitySystem.Editor.Generators
{
    public static class AbilityTypeGenerator
    {
        #region Constants
        private const string AbilityTypeFolder = "Assets/AbilitySystem/Scripts/Runtime/Enums";
        private const string AbilityTypeFilePath = "Assets/AbilitySystem/Scripts/Runtime/Enums/AbilityType.cs";
        private const string Indent = "    "; 
        #endregion

        #region Executes
        public static bool AddAbilityType(string abilityName)
        {
            if (!Directory.Exists(AbilityTypeFolder))
                Directory.CreateDirectory(AbilityTypeFolder);
            
            if (!File.Exists(AbilityTypeFilePath))
            {
                string content =
$@"namespace AbilitySystem.Scripts.Runtime.Enums
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
                return false;

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
                
            return true;
        }
        #endregion
    }
}