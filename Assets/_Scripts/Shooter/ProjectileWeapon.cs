using UnityEngine;
using UpgradeSystem;

public class ProjectileWeapon :IWeapon
{
    public GameObject projectilePrefab;
    public float speed = 20f;
    public int damage = 1;
    public float fireRate;
    
    private float lastShotTime;
    private IWeapon weaponImplementation;

    public ProjectileWeapon(GameObject projectilePrefab, float speed, int damage, float fireRate)
    {
        this.projectilePrefab = projectilePrefab;
        this.speed = speed;
        this.damage = damage;
        this.fireRate = fireRate;
    }


    public WeaponType Type => WeaponType.ProjectileShooter;

    public bool CanFire()
    {
        return Time.time - lastShotTime > fireRate;
    }

    public  void StartFire(Transform muzzlePoint, Vector3 targetPosition)
    {
        if(!CanFire()) return;
        
        if(projectilePrefab == null || muzzlePoint == null) return ;

        // Create projectile
        GameObject projectile = Object.Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation);

        // Add velocity
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null){
            Vector3 shootDir = (targetPosition - muzzlePoint.position).normalized;
            rb.linearVelocity = shootDir * speed;
        }

        // Set damage
        projectile.GetComponent<Projectile>()?.SetDamage(damage);
        
        lastShotTime = Time.time;
    }

    public void StopFire(){}
    
    
}