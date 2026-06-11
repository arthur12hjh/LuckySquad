using Attack;
using Item;
using UnityEngine;

public class TestMonster : MonoBehaviour, IDamageable
{
    public GameObject prefab;

    public float CriticalDamage()
    {
        throw new System.NotImplementedException();
    }

    public void Damaged(GameObject gameObject, SAttackData DamageStruct)
    {
        Debug.Log($"Damage : {DamageStruct.iDamage } \n" +
                  $"Hit Count: { DamageStruct.iHitCount } \n" + 
                  $"Type : { DamageStruct.AttackType.ToString() }");
    }

    public float GuardDamage()
    {
        throw new System.NotImplementedException();
    }

    public bool IsCritical()
    {
        throw new System.NotImplementedException();
    }

    public bool IsGuard()
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ItemFactory.AbstractCreateItem<OrbitingWeapon>(prefab, gameObject.transform, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
