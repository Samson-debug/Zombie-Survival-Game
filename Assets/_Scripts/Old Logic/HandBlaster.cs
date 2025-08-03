/*using System.Collections;
using UnityEngine;
using UpgradeSystem;

public class HandBlaster : MonoBehaviour
{
    [Header("Aim")]
    public float minAimDist = 10f;

    public float defaultAimDist = 10f;
    public Vector3 aimOffset = Vector3.up * 0.5f;
    public LayerMask aimMask = -1;

    [Header("Blaster Settings")]
    public float baseFireRate = 0.2f; // Base time between shots (seconds)

    public float projectileSpeed = 20f;
    public float projectileLifetime = 3f;
    public int damage = 1;

    [Header("Shake")]
    public float shakeDuration = 0.2f;

    public float shakeStrength = 0.2f;

    [Header("Blaster Components")]
    public Transform leftBlasterTransform;
    public Transform rightBlasterTransform;
    public GameObject projectilePrefab;
    public Transform leftMuzzlePoint;
    public Transform rightMuzzlePoint;
    
    [Header("Laser Components")]
    public LineRenderer leftLineRenderer;
    public LineRenderer rightLineRenderer;
    public float laserDamage = 1;
    public float laserRange = 50f;

    [Header("Effects")]
    public ParticleSystem leftMuzzleFlash;

    public ParticleSystem rightMuzzleFlash;
    public AudioSource leftAudioSource;
    public AudioSource rightAudioSource;
    public AudioClip shootSound;

    [Header("Input")]
    public float keyHeldStartTime = 0.3f;
    
    Camera cam;

    // Private variables
    private float currentFireRate;
    private float leftLastShotTime;
    private float rightLastShotTime;
    //private bool leftMouseKeyPressed;
    private bool leftMouseHeld = false;
    private float leftMouseHeldTime;
    //private bool rightMouseKeyPressed;
    private bool rightMouseHeld = false;
    private float rightMouseHeldTime;
    private bool leftShoot;
    private bool rightShoot;
    private Vector3 targetPos;

    // Upgrade tracking
    private float fireRateMultiplier = 1f;
    private Vector3 leftBlasterInitialPos;
    private Vector3 rightBlasterInitialPos;

    Coroutine leftBlasterRecoilCoroutine;
    Coroutine rightBlasterRecoilCoroutine;

    Laser leftLaser;
    Laser rightLaser;

    private void Awake()
    {
        cam = Camera.main;
        leftBlasterInitialPos = leftBlasterTransform.localPosition;
        rightBlasterInitialPos = rightBlasterTransform.localPosition;
    }

    void Start()
    {
        // Initialize fire rate
        currentFireRate = baseFireRate;

        // Validate components
        if (projectilePrefab == null){
            Debug.LogError("Projectile prefab not assigned to HandBlaster!");
        }

        if (leftMuzzlePoint == null || rightMuzzlePoint == null){
            Debug.LogError("Muzzle points not assigned to HandBlaster!");
        }
        
        leftLaser = new Laser(leftLineRenderer, laserDamage, laserRange);
        rightLaser = new Laser(rightLineRenderer, laserDamage, laserRange);
    }

    void Update()
    {
        if (UpgradeManager.Instance.Paused || GameManager.Instance.Paused) return;

        Aim();
        HandleInput();
        HandleShooting();
    }

    Vector3 hitPosition = Vector3.zero;

    void Aim()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, aimMask)){
            if (hit.distance < minAimDist){
                targetPos = cam.transform.position + cam.transform.forward * defaultAimDist;
                LeftDefaultAim(targetPos);
                RightDefaultAim(targetPos);
                return;
            }

            targetPos = hit.point;

            //Left blaster Rotation
            Vector3 leftTargetDir = targetPos - leftBlasterTransform.position;
            leftTargetDir += aimOffset;
            leftTargetDir.Normalize();
            Quaternion leftTargetRot = Quaternion.LookRotation(leftTargetDir);
            leftBlasterTransform.rotation =
                Quaternion.RotateTowards(leftBlasterTransform.rotation, leftTargetRot, 180f * Time.deltaTime);

            //Right Blaster Rotation
            Vector3 rightTargetDir = targetPos - rightBlasterTransform.position;
            rightTargetDir += aimOffset;
            rightTargetDir.Normalize();
            Quaternion rightTargetRot = Quaternion.LookRotation(leftTargetDir);
            rightBlasterTransform.rotation =
                Quaternion.RotateTowards(leftBlasterTransform.rotation, rightTargetRot, 180f * Time.deltaTime);
        }
        else{
            targetPos = cam.transform.position + cam.transform.forward * defaultAimDist;
            LeftDefaultAim(targetPos);
            RightDefaultAim(targetPos);
        }

        void LeftDefaultAim(Vector3 _targetPos)
        {
            Vector3 targetDir = _targetPos - leftBlasterTransform.position;
            targetDir += aimOffset;
            targetDir.Normalize();
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            leftBlasterTransform.rotation =
                Quaternion.RotateTowards(leftBlasterTransform.rotation, targetRot, 180f * Time.deltaTime);
        }

        void RightDefaultAim(Vector3 _targetPos)
        {
            Vector3 targetDir = _targetPos - rightBlasterTransform.position;
            targetDir += aimOffset;
            targetDir.Normalize();
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            rightBlasterTransform.rotation =
                Quaternion.RotateTowards(rightBlasterTransform.rotation, targetRot, 180f * Time.deltaTime);
        }
    }

    void HandleInput()
    {
        // Left mouse button for left hand blaster
        if (Input.GetMouseButtonDown(0)){
            //leftMouseKeyPressed = true;
        }
        else if (Input.GetMouseButton(0)){
            leftMouseHeldTime += Time.deltaTime;
        }
        else if (Input.GetMouseButtonUp(0)){
            if (leftMouseHeld)
                leftLaser.StopFiring();
            else
                leftShoot = true;
            //leftMouseKeyPressed = false;
            leftMouseHeld = false;
            leftMouseHeldTime = 0f;
        }

        // Right mouse button for right hand blaster
        if (Input.GetMouseButtonDown(1)){
            //rightMouseKeyPressed = true;
        }
        else if (Input.GetMouseButton(1)){
            rightMouseHeldTime += Time.deltaTime;
        }
        else if (Input.GetMouseButtonUp(1)){
            if(rightMouseHeld)
                rightLaser.StopFiring();
            else
                rightMouseHeld = true;
            //rightMouseKeyPressed = false;
            rightMouseHeld = false;
            rightMouseHeldTime = 0f;
        }

        leftMouseHeld = leftMouseHeldTime >= keyHeldStartTime;
        rightMouseHeld = rightMouseHeldTime >= keyHeldStartTime;
    }

    void HandleShooting()
    {
        // Calculate actual fire rate with upgrades
        float actualFireRate = baseFireRate / fireRateMultiplier;

        // Left hand blaster shooting
        if (leftMouseHeld){
            leftLaser.Fire(leftMuzzlePoint, targetPos, damage);
        }
        else if (leftShoot && !leftMouseHeld && Time.time - leftLastShotTime >= actualFireRate){
            ShootLeftBlaster();
            leftLastShotTime = Time.time;
        }

        if (rightMouseHeld){
            rightLaser.Fire(rightMuzzlePoint, targetPos, damage);
        }
        else if (rightShoot && !rightMouseHeld && Time.time - rightLastShotTime >= actualFireRate){
            ShootRightBlaster();
            rightLastShotTime = Time.time;
        }
        // Right hand blaster shooting
        /*if (rightMouseHeld && Time.time - rightLastShotTime >= actualFireRate){
            ShootRightBlaster();
            rightLastShotTime = Time.time;
        }#1#
    }

    void ShootLeftBlaster()
    {
        if (leftMuzzlePoint == null){
            leftShoot = false;
            return;
        }

        CreateProjectile(leftMuzzlePoint);
        Recoil(leftBlasterTransform, true);
        PlayMuzzleEffects(true); // true for left hand
        leftShoot = false;
    }

    void ShootRightBlaster()
    {
        if (rightMuzzlePoint == null){
            rightShoot = true;
            return;
        }

        CreateProjectile(rightMuzzlePoint);
        Recoil(rightBlasterTransform, false);
        PlayMuzzleEffects(false); // false for right hand
        rightShoot = true;
    }

    void CreateProjectile(Transform muzzlePoint)
    {
        if (projectilePrefab == null) return;

        // Create projectile at muzzle point
        GameObject projectile = Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation);

        // Add velocity to projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null){
            Vector3 shootDir = (targetPos - muzzlePoint.position).normalized;
            rb.linearVelocity = shootDir * projectileSpeed;
        }

        // Set projectile damage
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null){
            projectileScript.damage = damage;
        }

        // Destroy projectile after lifetime
        Destroy(projectile, projectileLifetime);
    }

    void PlayMuzzleEffects(bool isLeftHand)
    {
        // Play muzzle flash
        if (isLeftHand && leftMuzzleFlash != null){
            leftMuzzleFlash.Play();
        }
        else if (!isLeftHand && rightMuzzleFlash != null){
            rightMuzzleFlash.Play();
        }

        // Play shoot sound
        if (isLeftHand && leftAudioSource != null && shootSound != null){
            leftAudioSource.PlayOneShot(shootSound);
        }
        else if (!isLeftHand && rightAudioSource != null && shootSound != null){
            rightAudioSource.PlayOneShot(shootSound);
        }
    }

    // Upgrade methods (called by upgrade system)
    public void UpgradeFireRate(float percentageIncrease)
    {
        fireRateMultiplier += percentageIncrease / 100f;
        Debug.Log($"Fire rate upgraded! New multiplier: {fireRateMultiplier}");
    }

    public void UpgradeDamage(int damageIncrease)
    {
        damage += damageIncrease;
        Debug.Log($"Damage upgraded! New damage: {damage}");
    }

    // Getters for UI/stats
    public float GetCurrentFireRate()
    {
        return baseFireRate / fireRateMultiplier;
    }

    public int GetCurrentDamage()
    {
        return damage;
    }

    public float GetFireRateMultiplier()
    {
        return fireRateMultiplier;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hitPosition, 0.3f);
    }

    public void Recoil(Transform target, bool isLeft)
    {
        if (isLeft){
            if (leftBlasterRecoilCoroutine != null)
                StopCoroutine(leftBlasterRecoilCoroutine);
            leftBlasterRecoilCoroutine = StartCoroutine(DoRecoil(target, true));
        }
        else{
            if (rightBlasterRecoilCoroutine != null)
                StopCoroutine(rightBlasterRecoilCoroutine);
            rightBlasterRecoilCoroutine = StartCoroutine(DoRecoil(target, false));
        }

        IEnumerator DoRecoil(Transform target, bool isLeft)
        {
            Vector3 originalPos = isLeft ? leftBlasterInitialPos : rightBlasterInitialPos;
            Vector3 recoilPos = originalPos - transform.InverseTransformDirection(target.forward) * shakeStrength;

            // Move back
            float elapsed = 0f;
            while (elapsed < shakeDuration){
                target.localPosition = Vector3.Lerp(originalPos, recoilPos, elapsed / shakeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.localPosition = recoilPos;

            // Return forward
            elapsed = 0f;
            while (elapsed < shakeDuration){
                target.localPosition = Vector3.Lerp(recoilPos, originalPos, elapsed / shakeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.localPosition = originalPos; // ensure perfect reset
        }
    }
}*/