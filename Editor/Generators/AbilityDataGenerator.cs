using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Runtime.Generators
{
    public static class AbilityDataGenerator
    {
        private const string AbilityDataFolder = "Assets/Resources/AbilitySystem2/ScriptableObjects";
        private const string AbilityDataClassFolder = "Assets/AbilitySystem2/Scripts/Runtime/Data";
        private const string AbilityDataName = "Data";
        private const string AbilityDataNamespace = "AbilitySystem2.Scripts.Runtime.Data";
        private const string Indent = "    ";

        private static string _pendingClassName;
        private static string _pendingAssetPath;

        private static bool _waitingForCompilation;
        
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
{Indent}public sealed class {className} : AbilityData
{Indent}{{
{Indent}}}
}}";

            File.WriteAllText(classFilePath, content);

            Debug.Log("CLASS GENERATED: " + classFilePath);

            _pendingClassName = className;
            _pendingAssetPath = assetPath;

            AssetDatabase.Refresh();

            // Start polling
            _waitingForCompilation = true;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;

            return true;
        }

        private static void OnEditorUpdate()
        {
            // Wait until compile FINISHES
            if (_waitingForCompilation && !EditorApplication.isCompiling)
            {
                _waitingForCompilation = false;

                Debug.Log("<color=yellow>Compilation finished. Creating SO...</color>");
                
                CreateSOAfterCompile();
                
                EditorApplication.update -= OnEditorUpdate;
            }
        }

        private static void CreateSOAfterCompile()
        {
            string fullName = $"{AbilityDataNamespace}.{_pendingClassName}";

            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .FirstOrDefault(t => t.FullName == fullName);

            if (type == null)
            {
                Debug.LogError("TYPE STILL NULL → " + fullName);
                return;
            }

            ScriptableObject instance = ScriptableObject.CreateInstance(type);

            AssetDatabase.CreateAsset(instance, _pendingAssetPath);
            AssetDatabase.SaveAssets();

            Debug.Log("<color=green>SO CREATED: " + _pendingAssetPath + "</color>");
        }
        
        [MenuItem("Tools/Debug AbilitySystem Paths")]
        public static void DebugAbilitySystemPaths()
        {
            string[] abilityDataClass = Directory.GetFiles(Application.dataPath, "AbilityData.cs", SearchOption.AllDirectories);
            string[] abilitySystemFolder = Directory.GetDirectories(Application.dataPath, "AbilitySystem", SearchOption.AllDirectories);
            string[] resourcesFolders = Directory.GetDirectories(Application.dataPath, "Resources", SearchOption.AllDirectories);

            Debug.Log("=== AbilitySystem PATH DEBUG ===");

            foreach (var p in abilitySystemFolder)
                Debug.Log("AbilitySystem folder: " + p);

            foreach (var p in resourcesFolders)
                Debug.Log("Resources folder: " + p);

            foreach (var p in abilityDataClass)
                Debug.Log("AbilityData.cs: " + p);

            Debug.Log("IsCompiling currently: " + UnityEditor.EditorApplication.isCompiling);
        }

    }
}
