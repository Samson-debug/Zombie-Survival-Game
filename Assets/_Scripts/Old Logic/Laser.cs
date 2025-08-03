using UnityEngine;

// Laser weapon implementation
public class Laser
{
    private LineRenderer laser;
    private float damage;
    private float range;
    private float damageInterval = 0.5f; // Damage every 0.1 seconds
    private float lastDamageTime;
    private bool isFiring;
    private ParticleSystem hitEffect;
    
    public Laser(LineRenderer _laserRenderer, float laserDamage, float laserRange)
    {
        laser = _laserRenderer;
        damage = laserDamage;
        range = laserRange;
        
        // Initialize laser renderers
        if (laser != null)
        {
            laser.enabled = false;
            laser.positionCount = 2;
        }
    }
    
    public bool CanFire()
    {
        return true; // Laser can always fire when held
    }

    public void Fire(Transform muzzlePoint, Vector3 targetPosition, int weaponDamage)
    {
        isFiring = true;

        if (laser == null) return;

        laser.enabled = true;
        laser.SetPosition(0, muzzlePoint.position);

        // Raycast to find hit point
        Vector3 direction = (targetPosition - muzzlePoint.position).normalized;
        if (Physics.Raycast(muzzlePoint.position, direction, out RaycastHit hit, range)){
            laser.SetPosition(1, hit.point);

            // Check if hit object has health component
            var enemy = hit.collider.GetComponent<EnemyAi>();

            // Deal damage at intervals
            if (enemy != null && Time.time - lastDamageTime >= damageInterval){
                enemy.GetHit(Mathf.RoundToInt(damage));
                lastDamageTime = Time.time;
            }
        }
        else{
            laser.SetPosition(1, muzzlePoint.position + direction * range);
        }
    }
    
    public void StopFiring()
    {
        isFiring = false;
        if (laser != null) laser.enabled = false;
    }
    
    public void OnUpgrade(string upgradeType, float value)
    {
        if (upgradeType == "Damage")
        {
            damage += value;
        }
        else if (upgradeType == "Range")
        {
            range += value;
        }
    }
}