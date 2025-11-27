using System.IO;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Editor.Generators
{
    public static class AbilityClassGenerator
    {
        #region Constants
        private const string AbilityClassFolder = "Assets/AbilitySystem/Runtime/Abilities";
        private const string Indent = "    "; 
        #endregion

        #region Executes
        public static void CreateAbilityClass(string abilityName)
        {
            Debug.Log($"[AbilityClassGenerator] Requested ability class generate → {abilityName}");

            if (!Directory.Exists(AbilityClassFolder))
            {
                Directory.CreateDirectory(AbilityClassFolder);
                
                Debug.Log($"[AbilityClassGenerator] Created folder → {AbilityClassFolder}");
            }

            string className = $"{abilityName}Ability";
            string filePath = $"{AbilityClassFolder}/{className}.cs";
            string dataName = $"{abilityName}Data";

            if (File.Exists(filePath))
            {
                Debug.LogWarning($"[AbilityClassGenerator] SKIPPED. Class already exists → {filePath}");
                
                return;
            }

            Debug.Log($"[AbilityClassGenerator] Creating class file → {filePath}");

            string content =
$@"using AbilitySystem.Runtime.Data;

namespace AbilitySystem.Runtime.Abilities
{{
{Indent}public sealed class {className} : BaseAbility<{dataName}>
{Indent}{{
{Indent}{Indent}public override void Initialize({dataName} abilityData)
{Indent}{Indent}{{
{Indent}{Indent}{Indent}base.Initialize(abilityData);

{Indent}{Indent}{Indent}// TODO: {className} initialize logic here
{Indent}{Indent}}}

{Indent}{Indent}public override void Execute()
{Indent}{Indent}{{
{Indent}{Indent}{Indent}base.Execute();

{Indent}{Indent}{Indent}// TODO: {className} execute logic here
{Indent}{Indent}}}

{Indent}{Indent}public override void Cancel()
{Indent}{Indent}{{
{Indent}{Indent}{Indent}base.Cancel();

{Indent}{Indent}{Indent}// TODO: {className} cancel logic here
{Indent}{Indent}}}
{Indent}}}
}}";

            File.WriteAllText(filePath, content);
            
            AssetDatabase.Refresh();

            Debug.Log($"[AbilityClassGenerator] ✔ Created class → {className}");
        }
        #endregion
    }
}