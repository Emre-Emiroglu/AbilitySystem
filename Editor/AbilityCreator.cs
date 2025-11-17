using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Editor
{
    public sealed class AbilityCreator : EditorWindow
    {
        #region Core
        [MenuItem("Tools/Ability Creator")]
        private static void ShowWindow()
        {
            AbilityCreator window = GetWindow<AbilityCreator>();
            
            window.titleContent = new GUIContent("Ability Creator");
            window.minSize = new Vector2(480, 270);
            window.maxSize = new Vector2(960, 540);
            
            window.Show();
        }
        #endregion
    }
}