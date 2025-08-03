using UnityEngine;
using UpgradeSystem;
using Shooter;

namespace UpgradeSystem
{
    [CreateAssetMenu(fileName = "New Stat Upgrade", menuName = "Upgrade System/Stat Upgrade")]
    public class StatUpgrade : BaseUpgrade
    {
        [Header("Stat Modification")]
        public StatType statType;

        public bool isPercentage = true;
        public float value;

        public override void ApplyUpgrade(GameObject target)
        {
            var statManager = target.GetComponent<PlayerStatManager>();
            if (statManager != null){
                statManager.ApplyStatUpgrade(this);
            }
        }

        /*public override string GetFormattedDescription()
        {
            string prefix = value > 0 ? "+" : "";
            string suffix = isPercentage ? "%" : "";
            return description.Replace("{value}", $"{prefix}{value}{suffix}");
        }*/
    }
}

public abstract class WeaponStatUpgrade : BaseUpgrade
{
    public WeaponType weaponType;
    public WeaponStatType weaponStatType;
    
    public bool isPercentage = true;
    public float value;
    
    public override void ApplyUpgrade(GameObject target)
    {
        var statManager = target.GetComponent<WeaponManager>();
        if (statManager != null){
            //statManager.ApplyStatUpgrade(this);
        }
    }
}