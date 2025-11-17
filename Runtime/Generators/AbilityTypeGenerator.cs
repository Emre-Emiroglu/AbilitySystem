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
        #endregion

        #region Executes
        public static void AddAbilityType(string abilityName)
        {
            if (!Directory.Exists(EnumFolder))
                Directory.CreateDirectory(EnumFolder);
            
            if (!File.Exists(EnumFilePath))
                Debug.LogWarning($"AbilityType.cs not found at: {EnumFilePath}, creating new one.");

            string content = GetContent();
            
            File.WriteAllText(EnumFilePath, content);
            
            string[] lines = File.ReadAllLines(EnumFilePath);
            
            using StreamWriter writer = new StreamWriter(EnumFilePath);

            foreach (string line in lines)
            {
                if (line.Trim().Equals("}"))
                    writer.WriteLine($"{abilityName},");

                writer.WriteLine(line);
            }

            writer.Close();
            
            AssetDatabase.Refresh();
        }
        private static string GetContent()
        {
            const string content =
                @"namespace AbilitySystem.Scripts.Runtime.Enums
                {
                    public enum AbilityType
                    {
                    }
                }";

            return content;
        }
        #endregion
    }
}