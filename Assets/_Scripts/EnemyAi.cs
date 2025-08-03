using Shooter;
using UnityEngine;
using UnityEngine.AI;
using UpgradeSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAi : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealthPoints = 1;
    public float delayBetweenDamage = 0.5f;
    int currentHealthPoints;
    private float lastDamageTime;
    
    [Header("Chase")]
    public float chaseSpeed = 5f;
    public float stoppingDistance = 1.5f;
    public float rotationSpeed = 360f;

    [Header("Attack")]
    public float attackRadi = 0.5f;

    [Header("Death")]
    public GameObject orb;

    private readonly int attackAnimationHash = Animator.StringToHash("Attack");
    private readonly int hitAnimationHash = Animator.StringToHash("Hit");
    private readonly int deathAnimationHash = Animator.StringToHash("Dead");
    
    NavMeshAgent agent;
    Animator animator;

    Transform playerTransform;
    
    private bool isDead;
    private bool canAttack = true;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        agent.speed = chaseSpeed;
        agent.stoppingDistance = stoppingDistance;
        currentHealthPoints = maxHealthPoints;
    }

    private void OnEnable()
    {
        EnemyAnimationHelper animationHelper = GetComponentInChildren<EnemyAnimationHelper>();
        animationHelper.OnAttack += DoDamage;
        animationHelper.OnAttackComplete += () => { canAttack = true;};
        animationHelper.OnHitComplete += () => { canAttack = true;};
        animationHelper.OnDeath += SpawnOrb;
    }

    private void DoDamage()
    {
        if(!CanAttack()) return;
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward, attackRadi);
        
        if(hitColliders == null || hitColliders.Length == 0) return;

        Health playerHealth = null;
        
        foreach (var col in hitColliders){
            playerHealth = col.GetComponent<Health>();
            if(playerHealth != null) break;
        }

        playerHealth?.GetHit();
    }

    private void Update()
    {
        if(UpgradeManager.Instance.Paused) return;
        
        if(isDead) return;
        
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        
        //chase

        if (canAttack){
            if (dist <= stoppingDistance){
                
                if(CanAttack()){
                    canAttack = false;
                    animator.SetTrigger(attackAnimationHash);
                    
                }
                else{
                    Vector3 lookDir = (playerTransform.position - transform.position).normalized;
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
                }
            }
            else{
                if (NavMesh.SamplePosition(playerTransform.position, out NavMeshHit hit, 0.5f, NavMesh.AllAreas)){
                    agent.destination = hit.position;
                    agent.speed = chaseSpeed;
                }
            }
        }
    }

    private bool CanAttack()
    {
        Vector3 lookAtDir = playerTransform.position - transform.position;
        lookAtDir.Normalize();
        return Vector3.Dot(lookAtDir, transform.forward) > 0.9f; //facing towards player
    }

    public void GetHit(float hit)
    {
        if(isDead) return;
        if(Time.time - lastDamageTime < delayBetweenDamage) return;
        
        currentHealthPoints -= (int)hit;
        lastDamageTime = Time.time;

        if (currentHealthPoints <= 0){
            Collider collider = gameObject.GetComponent<Collider>();
            if(collider) collider.enabled = false;
            
            StopAgent();
            GameManager.Instance.EnemyDied();
            
            animator.transform.localPosition = Vector3.zero;
            animator.SetTrigger(deathAnimationHash);
            
            isDead = true;
            
            print("enemy is dead");
        }
        else{
            animator.SetTrigger(hitAnimationHash);
            canAttack = false;
        }
        
        
    }

    private void StopAgent()
    {
        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    private void SpawnOrb()
    {
        if(orb == null) return;
        
        Instantiate( orb, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

