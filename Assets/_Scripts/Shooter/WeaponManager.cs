using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UpgradeSystem;

namespace Shooter
{
    public class WeaponManager : MonoBehaviour
    {
        public GameObject projectile;
        private bool wasLeftHandFiring;
        private bool wasRightHandFiring;
        
        public Transform leftHand;
        public Transform rightHand;
        public Transform leftMuzzlePoint;
        public Transform rightMuzzlePoint;
        
        [Header("Aim")]
        public float minAimDist = 10f;

        public float defaultAimDist = 10f;
        public Vector3 aimOffset = Vector3.up * 0.5f;
        public LayerMask aimMask = -1;

        [Header("Weapons")]
        public List<IWeapon> leftHandWeapons = new List<IWeapon>();
        public List<IWeapon> rightHandWeapons = new();

        private Camera cam;
        private Vector3 targetPos;

        private float leftHeldTime;
        private float rightHeldTime;
        
        private void Awake()
        {
            cam = Camera.main;

            IWeapon leftWeapon = new ProjectileWeapon(projectile, 20f, 1, 0.2f);
            IWeapon rightWeapon = new ProjectileWeapon(projectile, 20f, 1, 0.2f);
            
            leftHandWeapons.Add(leftWeapon);
            rightHandWeapons.Add(rightWeapon);
        }

        void Update()
        {
            if (UpgradeManager.Instance.Paused || GameManager.Instance.Paused) return;

            Aim();
            HandleInput();
        }

        void Aim()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, aimMask)){
                if (hit.distance < minAimDist){
                    targetPos = cam.transform.position + cam.transform.forward * defaultAimDist;
                }
                else{
                    targetPos = hit.point;
                }
            }
            else{
                targetPos = cam.transform.position + cam.transform.forward * defaultAimDist;
            }
            
            Aim(leftHand, targetPos);
            Aim(rightHand, targetPos);

            void Aim(Transform weaponTransform, Vector3 targetPosition)
            {
                Vector3 targetDir = targetPosition - weaponTransform.position;
                targetDir += aimOffset;
                targetDir.Normalize();
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                weaponTransform.rotation =
                    Quaternion.RotateTowards(weaponTransform.rotation, targetRot, 180f * Time.deltaTime);
            }
        }

        void HandleInput()
        {
            // Left mouse button for left hand blaster
            if (Input.GetMouseButtonDown(0)){
                leftHeldTime = 0f;
            }
            else if (Input.GetMouseButton(0)){
                leftHeldTime += Time.deltaTime;
                
                if(leftHeldTime >= 0.3f)
                    foreach (var weapon in leftHandWeapons){
                        if (weapon.CanFire() && weapon is ContinuousWeapon){
                            weapon.StartFire(leftMuzzlePoint, targetPos);
                            wasLeftHandFiring = true;
                        }
                    }
            }
            else if (Input.GetMouseButtonUp(0)){
                if (!wasLeftHandFiring){
                    foreach (var weapon in leftHandWeapons){
                        if (weapon.CanFire() && weapon is ProjectileWeapon){
                            weapon.StartFire(leftMuzzlePoint, targetPos);
                        }
                    }
                }
                else{
                    foreach (var weapon in leftHandWeapons){
                        if (weapon is ContinuousWeapon) weapon.StopFire();
                    }
                }
                
                wasLeftHandFiring = false;
            }

            // Left mouse button for left hand blaster
            if (Input.GetMouseButtonDown(1)){
                rightHeldTime = 0f;
            }
            else if (Input.GetMouseButton(1)){
                rightHeldTime += Time.deltaTime;
                
                if(rightHeldTime >= 0.3f)
                    foreach (var weapon in rightHandWeapons){
                        if (weapon.CanFire() && weapon is ContinuousWeapon){
                            weapon.StartFire(rightMuzzlePoint, targetPos);
                            wasRightHandFiring = true;
                        }
                    }
            }
            else if (Input.GetMouseButtonUp(1)){
                if (!wasRightHandFiring){
                    foreach (var weapon in rightHandWeapons){
                        if (weapon.CanFire() && weapon is ProjectileWeapon){
                            weapon.StartFire(rightMuzzlePoint, targetPos);
                        }
                    }
                }
                else{
                    foreach (var weapon in rightHandWeapons){
                        if (weapon is ContinuousWeapon) weapon.StopFire();
                    }
                }
                
                wasRightHandFiring = false;
            }
        }

        public void AddWeapon(IWeapon _leftWeapon, IWeapon _rightWeapon)
        {
            if(_leftWeapon == null || _rightWeapon == null) return;

            leftHandWeapons.Add(_leftWeapon);
            rightHandWeapons.Add(_rightWeapon);
        }

        public void UpgrageWeaponStat(WeaponStatUpgrade _upgrade)
        {
            if (!HasTypeOfWeapon(_upgrade.weaponType)) return;

            var leftWeapon = leftHandWeapons.Find(w => w.Type == _upgrade.weaponType);
            var rightWeapon = rightHandWeapons.Find(w => w.Type == _upgrade.weaponType);
            
            //Switch
        }

        public bool HasTypeOfWeapon(WeaponType _weapType)
        {
            return leftHandWeapons.Find(weapon => weapon.Type == _weapType) != null;
        }
        
        /*// Upgrade methods
        public void UpgradeWeaponFireRate(int weaponIndex, float percentageIncrease)
        {
            if (weaponIndex >= 0 && weaponIndex < weapons.Count){
                weapons[weaponIndex].fireRateMultiplier += percentageIncrease / 100f;
                Debug.Log(
                    $"Weapon {weapons[weaponIndex].weaponData.weaponName} fire rate upgraded! New multiplier: {weapons[weaponIndex].fireRateMultiplier}");
            }
        }

        public void UpgradeWeaponDamage(int weaponIndex, float percentageIncrease)
        {
            if (weaponIndex >= 0 && weaponIndex < weapons.Count){
                weapons[weaponIndex].damageMultiplier += percentageIncrease / 100f;
                Debug.Log(
                    $"Weapon {weapons[weaponIndex].weaponData.weaponName} damage upgraded! New multiplier: {weapons[weaponIndex].damageMultiplier}");
            }
        }

        public void UpgradeAllWeaponsFireRate(float percentageIncrease)
        {
            foreach (var weapon in weapons){
                weapon.fireRateMultiplier += percentageIncrease / 100f;
            }
        }

        public void UpgradeAllWeaponsDamage(float percentageIncrease)
        {
            foreach (var weapon in weapons){
                weapon.damageMultiplier += percentageIncrease / 100f;
            }
        }

        // Getters
        public WeaponInstance GetWeapon(int index)
        {
            if (index >= 0 && index < weapons.Count)
                return weapons[index];
            return null;
        }

        public int GetWeaponCount()
        {
            return weapons.Count;
        }*/
    }
}
