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
        private static void ShowWindow()
        {
            AbilityCreator window = GetWindow<AbilityCreator>();
            
            window.titleContent = new GUIContent("Ability Creator");
            
            window.minSize = new Vector2(240, 135);
            window.maxSize = new Vector2(480, 270);
            
            window.Show();
        }
        private void OnGUI()
        {
            GUILayout.Label("Create New Ability", EditorStyles.boldLabel);

            _abilityName = EditorGUILayout.TextField("Ability Name", _abilityName);

            if (!GUILayout.Button("Create Ability"))
                return;
            
            if (string.IsNullOrWhiteSpace(_abilityName))
            {
                Debug.LogError("Ability name cannot be empty!");
                    
                return;
            }

            string cleanName = _abilityName.Replace(" ", "");
            
            AbilityTypeGenerator.AddAbilityType(cleanName);
            AbilityClassGenerator.CreateAbilityClass(cleanName);
                
            AbilityData data = CreateAbilityData(cleanName);
            
            Debug.Log($"{cleanName} Ability created successfully!");
        }
        #endregion

        #region Executes
        private static AbilityData CreateAbilityData(string abilityName)
        {
            AbilityData data = CreateInstance<AbilityData>();
            
            data.AbilityName = abilityName;

            string path = $"Assets/AbilitySystem/ScriptableObjects/{abilityName}Data.asset";
            
            AssetDatabase.CreateAsset(data, path);
            
            AssetDatabase.SaveAssets();

            return data;
        }
        #endregion
    }
}