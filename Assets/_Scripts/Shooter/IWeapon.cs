using UnityEngine;
using UpgradeSystem;

public interface IWeapon
{
    public WeaponType Type { get; }
    public bool CanFire();
    public void StartFire(Transform muzzlePoint, Vector3 targetPosition);
    public void StopFire();
}