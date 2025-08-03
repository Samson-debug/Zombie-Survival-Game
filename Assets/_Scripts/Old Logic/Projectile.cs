using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;
    
    private float currentLifetime = 0f;
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if(currentLifetime >= lifeTime)
            Destroy(gameObject);
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out EnemyAi enemy)){
            enemy.GetHit(damage);
        }
        
        Destroy(gameObject);
    }
}