using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    public class PlayerStatManager : MonoBehaviour
    {
        [Header("Base Stats")]
        public float baseSpeed = 5f;
        
        [Header("Current Stats")]
        public float currentSpeed;
        
        // Stat modifiers
        private Dictionary<StatType, float> flatModifiers = new Dictionary<StatType, float>();
        private Dictionary<StatType, float> percentageModifiers = new Dictionary<StatType, float>();

        // Events
        public static event Action<StatType, float> OnStatChanged;

        private void Start()
        {
            InitializeStats();
        }

        private void InitializeStats()
        {
            // Initialize modifier dictionaries
            foreach (StatType statType in System.Enum.GetValues(typeof(StatType))){
                flatModifiers[statType] = 0f;
                percentageModifiers[statType] = 0f;
            }

            RecalculateStats();
        }

        public void ApplyStatUpgrade(StatUpgrade upgrade)
        {
            if (upgrade.isPercentage){
                if(percentageModifiers.ContainsKey(upgrade.statType))
                    percentageModifiers[upgrade.statType] += upgrade.value;
            }
            else{
                flatModifiers[upgrade.statType] += upgrade.value;
            }

            RecalculateStats();
        }

        private void RecalculateStats()
        {
            currentSpeed = CalculateStat(StatType.Speed, baseSpeed);

            // Notify listeners
            OnStatChanged?.Invoke(StatType.Speed, currentSpeed);
        }

        private float CalculateStat(StatType statType, float baseStat)
        {
            float flat = flatModifiers[statType];
            float percentage = percentageModifiers[statType];

            return (baseStat + flat) * (1f + percentage / 100f);
        }

        public float GetStat(StatType statType)
        {
            return statType switch
            {
                StatType.Speed => currentSpeed,
                _ => 0f
            };
        }
    }
}