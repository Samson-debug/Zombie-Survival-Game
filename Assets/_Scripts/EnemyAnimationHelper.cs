using System;
using UnityEngine;

public class EnemyAnimationHelper : MonoBehaviour
{
    public Action OnAttack;
    public Action OnAttackComplete;
    public Action OnHitComplete;
    public Action OnDeath;

    public void Attack()
    {
        OnAttack?.Invoke();
    }
    public void AttackComplete()
    {
        OnAttackComplete?.Invoke();
    }

    public void HitComplete()
    {
        OnHitComplete?.Invoke();
    }

    public void DeathComplete()
    {
        OnDeath?.Invoke();
    }
}