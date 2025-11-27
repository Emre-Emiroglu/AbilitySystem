using UnityEngine;

namespace AbilitySystem.Runtime.Data
{
    public abstract class AbilityData : ScriptableObject
    {
        #region Fields
        [Header("Ability Data Fields")]
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