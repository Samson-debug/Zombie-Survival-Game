using UnityEngine;
using Shooter;

namespace UpgradeSystem
{
    [CreateAssetMenu(fileName = "New Weapon Upgrade", menuName = "Upgrade System/Weapon Upgrade")]
    public class WeaponUpgrade : BaseUpgrade
    {
        public WeaponType weaponType;

        [Header("Projectile Shooter")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 20f;
        public int projectileDamage = 1;
        public float timeBetweenShot = 0.2f;

        [Header("Continuous Shooter")]
        public LineRenderer lineRenderer;

        public float range = 50f;
        public float laserDamage = 1f;
        public float timeDelayBetweenDamage = 0.2f;
        public LayerMask targetMask;

        public override void ApplyUpgrade(GameObject target)
        {
            var manager = target.GetComponent<WeaponManager>();
            if (manager == null){
                Debug.LogError("Player don't have any weapon manager");
                return;
            }

            IWeapon leftWeapon;
            IWeapon rightWeapon;

            if (weaponType == WeaponType.ProjectileShooter){
                leftWeapon = new ProjectileWeapon(projectilePrefab, projectileSpeed, projectileDamage, timeBetweenShot);
                rightWeapon =
                    new ProjectileWeapon(projectilePrefab, projectileSpeed, projectileDamage, timeBetweenShot);
            }
            else{
                LineRenderer leftLaser = Instantiate(lineRenderer.gameObject, Vector3.zero, Quaternion.identity)
                    .GetComponent<LineRenderer>();
                LineRenderer rightLaser = Instantiate(lineRenderer.gameObject, Vector3.zero, Quaternion.identity)
                    .GetComponent<LineRenderer>();
                leftWeapon = new ContinuousWeapon(leftLaser, range, laserDamage, timeDelayBetweenDamage, targetMask);
                rightWeapon = new ContinuousWeapon(rightLaser, range, laserDamage, timeDelayBetweenDamage, targetMask);
            }

            manager.AddWeapon(leftWeapon, rightWeapon);
        }
    }
}