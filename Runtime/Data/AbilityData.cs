using UnityEngine;

namespace AbilitySystem.Runtime.Data
{
    public sealed class AbilityData : ScriptableObject
    {
        #region Fields
        [Header("Ability Fields")]
        [SerializeField] private string abilityName;
        #endregion

        #region Properities
        public string AbilityName
        {
            get => abilityName;
            set => abilityName = value;
        }
        #endregion
    }
}