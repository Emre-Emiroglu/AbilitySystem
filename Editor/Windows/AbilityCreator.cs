using AbilitySystem.Runtime.Data;
using AbilitySystem.Runtime.Generators;
using AbilitySystem.Runtime.Managers;
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

            if (DrawCreateButton())
                return;
            
            if (CheckAbilityNameIsEmpty())
                return;

            string sanitizedAbilityName = GetSanitizedAbilityName();

            if (!AbilityTypeGenerator.AddAbilityType(sanitizedAbilityName))
                return;
            
            if (!AbilityClassGenerator.CreateAbilityClass(sanitizedAbilityName))
                return;
            
            if (!AbilityDataGenerator.CreateAbilityData(sanitizedAbilityName))
                return;

            Debug.Log($"{sanitizedAbilityName} Ability created successfully!");
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
        private static bool DrawCreateButton() => !GUILayout.Button("Create Ability");
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