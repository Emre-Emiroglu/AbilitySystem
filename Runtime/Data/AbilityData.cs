using UnityEngine;

namespace AbilitySystem.Runtime.Data
{
    public abstract class AbilityData : ScriptableObject
    {
        #region Fields
        [Header("Ability Fields")]
        [SerializeField] private string abilityName;
        #endregion

        #region Getters
        public string AbilityName => abilityName;
        #endregion
    }
}