using UnityEngine;
using Attack;

public interface IDamageable
{
    void    Damaged(GameObject gameObject, SAttackData DamageStruct);

    float   GuardDamage();
    bool    IsGuard();

    float   CriticalDamage();
    bool    IsCritical();
}
