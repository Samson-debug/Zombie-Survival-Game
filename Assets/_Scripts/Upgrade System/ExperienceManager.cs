using System;
using UnityEngine;

namespace UpgradeSystem
{
    public class ExperienceManager : MonoBehaviour
    {
        [Header("Experience Settings")]
        public float currentXP = 0f;

        public float baseXPRequired = 100f;
        public float xpScalingFactor = 1.2f;
        public float timeXPGain = 1f;
        public float timeXPInterval = 1f;

        public int currentLevel = 1;
        public float xpRequiredForNextLevel;

        // Events
        public static event Action<int> OnLevelUp;
        public static event Action<float, float> OnXPChanged;

        private void Start()
        {
            xpRequiredForNextLevel = baseXPRequired;
            InvokeRepeating(nameof(AddTimeXP), timeXPInterval, timeXPInterval);
        }

        private void AddTimeXP()
        {
            AddXP(timeXPGain);
        }

        public void AddXP(float xpAmount)
        {
            currentXP += xpAmount;
            OnXPChanged?.Invoke(currentXP, xpRequiredForNextLevel);

            CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            while (currentXP >= xpRequiredForNextLevel){
                currentXP -= xpRequiredForNextLevel;
                currentLevel++;
                xpRequiredForNextLevel = baseXPRequired * Mathf.Pow(xpScalingFactor, currentLevel - 1);

                OnLevelUp?.Invoke(currentLevel);
                OnXPChanged?.Invoke(currentXP, xpRequiredForNextLevel);
            }
        }
    }
}