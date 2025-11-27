using AbilitySystem.Editor.Generators;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Editor.Windows
{
    public sealed class AbilityCreator : EditorWindow
    {
        #region Fields
        private string _abilityName;
        #endregion
        
        #region Core
        [MenuItem("Tools/Ability Creator")]
        private static void ShowWindow() => DrawEditor();
        private void OnGUI()
        {
            DrawAbilityNameField();
            
            GUILayout.Space(8);

            if (GenerateAbilityClasses())
                return;
            
            GUILayout.Space(4);

            CreateAbilityScriptableObject();
        }
        #endregion

        #region Executes
        private static void DrawEditor()
        {
            AbilityCreator window = GetWindow<AbilityCreator>();
            
            window.titleContent = new GUIContent("Ability Creator");
            
            window.minSize = new Vector2(240, 135);
            window.maxSize = new Vector2(480, 270);
            
            window.Show();
        }
        private void DrawAbilityNameField()
        {
            GUILayout.Label("Create New Ability", EditorStyles.boldLabel);

            _abilityName = EditorGUILayout.TextField("Ability Name", _abilityName);
        }
        private bool GenerateAbilityClasses()
        {
            if (!DrawGenerateAbilityClassesButton())
                return false;
            
            if (CheckAbilityNameIsEmpty())
                return true;
            
            string sanitizedAbilityName = GetSanitizedAbilityName();
                
            AbilityDataGenerator.CreateAbilityData(sanitizedAbilityName);

            AbilityClassGenerator.CreateAbilityClass(sanitizedAbilityName);
            
            Debug.Log($"{sanitizedAbilityName} ability classes generated.");

            return false;
        }
        private void CreateAbilityScriptableObject()
        {
            if (!DrawCreateScriptableObjectButton())
                return;
            
            if (CheckAbilityNameIsEmpty())
                return;
            
            string sanitizedAbilityName = GetSanitizedAbilityName();

            AbilityDataGenerator.CreateSoFromMenu(sanitizedAbilityName);

            Debug.Log($"{sanitizedAbilityName} ScriptableObject created.");
        }
        private static bool DrawGenerateAbilityClassesButton() => GUILayout.Button("Generate Ability Classes");
        private static bool DrawCreateScriptableObjectButton() => GUILayout.Button("Create ScriptableObject");
        private bool CheckAbilityNameIsEmpty()
        {
            if (!string.IsNullOrWhiteSpace(_abilityName))
                return false;
            
            Debug.LogError("Ability name cannot be empty!");

            return true;
        }
        private string GetSanitizedAbilityName() => _abilityName.Trim().Replace(" ", string.Empty);
        #endregion
    }
}