using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Editor.Windows
{
    public sealed class AbilityCreator : EditorWindow
    {
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
        #endregion
    }
}