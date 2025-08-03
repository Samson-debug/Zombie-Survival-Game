using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    public class UpgradeApplier : MonoBehaviour
    {
        [Header("Applied Upgrades")]
        public List<BaseUpgrade> appliedUpgrades = new List<BaseUpgrade>();

        public Dictionary<BaseUpgrade, int> upgradeStacks = new Dictionary<BaseUpgrade, int>();

        // Events
        public static event Action<BaseUpgrade> OnUpgradeApplied;

        private GameObject player;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Start()
        {
            upgradeStacks = new Dictionary<BaseUpgrade, int>();
        }

        public void ApplyUpgrade(BaseUpgrade upgrade)
        {
            if (upgrade == null) return;

            // Check stacking
            if (upgradeStacks.ContainsKey(upgrade)){
                if (upgradeStacks[upgrade] >= upgrade.maxStacks)
                    return; // Max stacks reached

                upgradeStacks[upgrade]++;
            }
            else{
                upgradeStacks[upgrade] = 1;
                appliedUpgrades.Add(upgrade);
            }

            // Apply the upgrade
            upgrade.ApplyUpgrade(player);

            OnUpgradeApplied?.Invoke(upgrade);

            Debug.Log($"Applied upgrade: {upgrade.upgradeName} (Stack: {upgradeStacks[upgrade]})");
        }

        public bool HasUpgrade(BaseUpgrade upgrade)
        {
            return appliedUpgrades.Contains(upgrade);
        }
    }
}