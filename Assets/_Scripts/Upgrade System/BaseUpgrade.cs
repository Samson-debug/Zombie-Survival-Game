// ====================
// CORE DATA STRUCTURES
// ====================

using UnityEngine;


namespace UpgradeSystem
{

    [CreateAssetMenu(fileName = "New Base Upgrade", menuName = "Upgrade System/Base Upgrade")]
    public abstract class BaseUpgrade : ScriptableObject
    {
        [Header("Basic Info")]
        public string upgradeName;

        public string description;
        //public Sprite icon;
        public UpgradeType upgradeType;
        public bool isStackable = true;
        public int maxStacks = 1;

        [Header("Rarity")]
        public float weight = 1f; // For random selection

        //public Color rarityColor = Color.white;

        public abstract void ApplyUpgrade(GameObject target);
        //public abstract string GetFormattedDescription();
    }
    
    public enum UpgradeType
    {
        Stat,
        Weapon,
        WeaponStat
    }
    
    public enum StatType
    {
        Speed
    }

    public enum WeaponStatType
    {
        FireRate,
        Damage
    }

    public enum WeaponType
    {
        ProjectileShooter,
        ContinuousShooter,
    }
}