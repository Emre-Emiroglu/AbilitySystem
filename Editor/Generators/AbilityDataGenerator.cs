using System.IO;
using UnityEditor;

namespace AbilitySystem.Editor.Generators
{
    public static class AbilityDataGenerator
    {
        #region Constants
        private const string AbilityDataFolder = "Assets/Resources/AbilitySystem/ScriptableObjects";
        private const string AbilityDataClassFolder = "Assets/AbilitySystem/Scripts/Runtime/Data";
        private const string AbilityDataName = "Data";
        private const string AbilityDataNamespace = "AbilitySystem.Scripts.Runtime.Data";
        private const string Indent = "    ";
        #endregion

        #region Executes
        public static bool CreateAbilityData(string abilityName)
        {
            if (!Directory.Exists(AbilityDataFolder))
                Directory.CreateDirectory(AbilityDataFolder);

            if (!Directory.Exists(AbilityDataClassFolder))
                Directory.CreateDirectory(AbilityDataClassFolder);

            string className = $"{abilityName}{AbilityDataName}";
            string classFilePath = $"{AbilityDataClassFolder}/{className}.cs";
            string assetPath = $"{AbilityDataFolder}/{className}.asset";

            if (File.Exists(classFilePath) || File.Exists(assetPath))
                return false;

            string content =
$@"using AbilitySystem.Runtime.Data;

namespace {AbilityDataNamespace}
{{
{Indent}[CreateAssetMenu(fileName = ""{className}"", menuName = ""AbilitySystem/AbilityData/{className}"")]
{Indent}public sealed class {className} : AbilityData
{Indent}{{
{Indent}}}
}}";

            File.WriteAllText(classFilePath, content);

            AssetDatabase.Refresh();
            
            return true;
        }
        #endregion        
    }
}
