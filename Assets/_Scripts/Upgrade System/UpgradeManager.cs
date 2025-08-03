using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    public class UpgradeManager : MonoBehaviour
    {
        //Singleton
        public static UpgradeManager Instance{get; private set;}
        [Header("References")]
        public UpgradeDatabase upgradeDatabase;

        public UpgradeApplier upgradeApplier;
        public UpgradeUI upgradeUI;
        private UpgradeSelector upgradeSelector;
        
        public bool Paused {get; private set;}

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else{
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // Subscribe to events
            ExperienceManager.OnLevelUp += HandleLevelUp;

            if (upgradeApplier == null)
                upgradeApplier = GetComponent<UpgradeApplier>();
            
            upgradeSelector = new UpgradeSelector(upgradeDatabase, upgradeApplier);

            Time.timeScale = 1f;
        }

        private void Update()
        {
            if(!Paused) return;

            if (Input.GetMouseButtonDown(0))
                upgradeUI.SelectUpgrade(0);
            else if(Input.GetMouseButtonDown(1))
                upgradeUI.SelectUpgrade(1);
        }

        private void OnDestroy()
        {
            ExperienceManager.OnLevelUp -= HandleLevelUp;
        }

        private void HandleLevelUp(int newLevel)
        {
            Time.timeScale = 0f;
            Paused = true;

            // Get upgrade options
            List<BaseUpgrade> options = upgradeSelector.GetUpgradeOptions(newLevel);

            // Show upgrade UI
            if (upgradeUI != null){
                upgradeUI.ShowUpgradeOptions(options, SelectUpgrade);
            }
            else{
                Debug.LogError("UpgradeUI is not assigned!");
            }
        }

        public void SelectUpgrade(BaseUpgrade selectedUpgrade)
        {
            if (selectedUpgrade != null){
                upgradeApplier.ApplyUpgrade(selectedUpgrade);
            }
            // Resume game
            Time.timeScale = 1f;
            Paused = false;

            // Hide upgrade UI
            if (upgradeUI != null){
                upgradeUI.HideUpgradeOptions();
            }
        }
    }
}