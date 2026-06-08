using UnityEngine;
using Attack;

public interface DamageInterface
{
    void    Dagmed(GameObject gameObject, SAttackData DamageStruct);

    float   GuardDamage();
    bool    IsGuard();

    float   CriticalDamage();
    bool    IsCritical();
}
