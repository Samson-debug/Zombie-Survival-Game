using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    public class UpgradeSelector
    {
        [Header("References")]
        public UpgradeDatabase upgradeDatabase;

        [Header("Selection Settings")]
        private int maxUpgradeOptions = 2;
        
        UpgradeApplier upgradeApplier;

        private List<BaseUpgrade> availableUpgrades = new List<BaseUpgrade>();
        private List<BaseUpgrade> selectedUpgrades = new List<BaseUpgrade>();

        public UpgradeSelector(UpgradeDatabase upgradeDatabase, UpgradeApplier upgradeApplier)
        {
            this.upgradeDatabase = upgradeDatabase;
            this.upgradeApplier = upgradeApplier;
        }
        
        public List<BaseUpgrade> GetUpgradeOptions(int level)
        {
            selectedUpgrades.Clear();

            // Check for level-specific upgrades
            var levelConfig = upgradeDatabase.levelSpecificUpgrades.Find(config => config.level == level);

            if (levelConfig != null){
                // Handle rigged upgrades
                if (levelConfig.leftIsRandom)
                    selectedUpgrades.Add(GetRandomUpgrade());
                else
                    selectedUpgrades.Add(levelConfig.leftOption);

                if (levelConfig.rightIsRandom)
                    selectedUpgrades.Add(GetRandomUpgrade());
                else
                    selectedUpgrades.Add(levelConfig.rightOption);
            }
            else{
                // Random selection
                for (int i = 0; i < maxUpgradeOptions; i++){
                    selectedUpgrades.Add(GetRandomUpgrade());
                }
            }

            return selectedUpgrades;
        }

        private BaseUpgrade GetRandomUpgrade()
        {
            availableUpgrades.Clear();

            // Filter available upgrades (not at max stacks)
            foreach (var upgrade in upgradeDatabase.AllUpgrades){
                if (CanSelectUpgrade(upgrade)){
                    availableUpgrades.Add(upgrade);
                }
            }

            if (availableUpgrades.Count == 0)
                return null;

            // Weighted selection
            return SelectWeightedRandom(availableUpgrades);
        }

        private bool CanSelectUpgrade(BaseUpgrade upgrade)
        {
            if (!upgrade.isStackable){
                if (upgradeApplier != null && upgradeApplier.HasUpgrade(upgrade))
                    return false;
            }

            return true;
        }

        private BaseUpgrade SelectWeightedRandom(List<BaseUpgrade> upgrades)
        {
            float totalWeight = 0f;
            foreach (var upgrade in upgrades){
                totalWeight += upgrade.weight;
            }

            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (var upgrade in upgrades){
                currentWeight += upgrade.weight;
                if (randomValue <= currentWeight){
                    return upgrade;
                }
            }

            return upgrades[0]; // Fallback
        }
    }
}