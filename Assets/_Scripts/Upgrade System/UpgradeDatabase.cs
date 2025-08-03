using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    [CreateAssetMenu(fileName = "Upgrade Database", menuName = "Upgrade System/Upgrade Database")]
    public class UpgradeDatabase : ScriptableObject
    {
        [Header("All Available Upgrades")]
        public List<BaseUpgrade> AllUpgrades{ get; private set; }

        [Header("Upgrade Categories")]
        public List<BaseUpgrade> statUpgrades = new List<BaseUpgrade>();

        public List<BaseUpgrade> weaponUpgrades = new List<BaseUpgrade>();
        //public List<BaseUpgrade> itemUpgrades = new List<BaseUpgrade>();

        [Header("Level Specific Upgrades")]
        public List<LevelUpgradeConfig> levelSpecificUpgrades = new List<LevelUpgradeConfig>();

        private void OnValidate()
        {
            if(AllUpgrades == null) AllUpgrades = new List<BaseUpgrade>();
            AllUpgrades.Clear();
            
            AllUpgrades.AddRange(statUpgrades);
            AllUpgrades.AddRange(weaponUpgrades);
        }

        public List<BaseUpgrade> GetUpgradesByType(UpgradeType type)
        {
            return type switch
            {
                UpgradeType.Stat => statUpgrades,
                UpgradeType.Weapon => weaponUpgrades,
                _ => AllUpgrades
            };
        }

        public BaseUpgrade GetUpgradeByName(string upgradeName)
        {
            return AllUpgrades.Find(upgrade => upgrade.upgradeName == upgradeName);
        }
    }
    
    [System.Serializable]
    public class LevelUpgradeConfig
    {
        public int level;
        public BaseUpgrade leftOption;
        public BaseUpgrade rightOption;
        public bool leftIsRandom = false;
        public bool rightIsRandom = false;
    }
}