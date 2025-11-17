using System.IO;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem.Runtime.Generators
{
    public static class AbilityClassGenerator
    {
        #region Constants
        private const string AbilityFolder = "Assets/AbilitySystem/Scripts/Runtime/Abilities";
        #endregion

        #region Executes
        public static void CreateAbilityClass(string abilityName)
        {
            if (!Directory.Exists(AbilityFolder))
                Directory.CreateDirectory(AbilityFolder);

            string className = $"{abilityName}Ability";
            string filePath = $"{AbilityFolder}/{className}.cs";

            if (File.Exists(filePath))
            {
                Debug.LogWarning($"{className}.cs already exists!");
                
                return;
            }

            string content = GetContent(className);

            File.WriteAllText(filePath, content);
            
            AssetDatabase.Refresh();
        }
        private static string GetContent(string className)
        {
            string content =
                $@"using AbilitySystem.Runtime.Abilities;
                using AbilitySystem.Runtime.Data;

                namespace AbilitySystem.Scripts.Runtime.Abilities
                {{
                    public sealed class {className} : BaseAbility
                    {{
                        public override void Initialize(AbilityData abilityData)
                        {{
                            base.Initialize(abilityData);

                            // TODO: {className} initialize logic here
                        }}

                        public override void Execute()
                        {{
                            base.Execute();

                            // TODO: {className} execute logic here
                        }}

                        public override void Cancel()
                        {{
                            base.Cancel();

                            // TODO: {className} cancel logic here
                        }}
                    }}
                }}";
            
            return content;
        }
        #endregion
        
    }
}