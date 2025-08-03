using Shooter;
using UnityEngine;
using UpgradeSystem;

public class ContinuousWeapon : IWeapon
{
    public LineRenderer lineRenderer;
    public float range = 50f;
    public float damage = 1f;
    public LayerMask targetMask;
    public float timeDelayBetweenDamage;
    private float lastDamageGivenTime;
    
    public ContinuousWeapon(
        LineRenderer lineRenderer, float range, float damage, float timeDelayBetweenDamage, LayerMask targetMask)
    {
        this.lineRenderer = lineRenderer;
        lineRenderer.enabled = false;
        
        this.range = range;
        this.damage = damage;
        this.timeDelayBetweenDamage = timeDelayBetweenDamage;
        this.targetMask = targetMask;
    }

    public WeaponType Type => WeaponType.ContinuousShooter;
    public bool CanFire() => true;

    public void StartFire(Transform muzzlePoint, Vector3 targetPosition)
    {
        if (lineRenderer == null || muzzlePoint == null) return;
        
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, muzzlePoint.position);

        // Raycast to find hit point
        Vector3 shootDirection = (targetPosition - muzzlePoint.position).normalized;
        if (Physics.Raycast(muzzlePoint.position, shootDirection, out RaycastHit hit, range, targetMask))
        {
            lineRenderer.SetPosition(1, hit.point);
            if (lastDamageGivenTime + timeDelayBetweenDamage > Time.time) return;
            
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null){
                damageable.GetHit(damage);
                lastDamageGivenTime = Time.time;
            }
        }
        else{
            lineRenderer.SetPosition(1, muzzlePoint.position + shootDirection * range);
        }
    }

    public void StopFire()
    {
        if (lineRenderer == null) return;
        lineRenderer.enabled = false;
    }
    
    
}