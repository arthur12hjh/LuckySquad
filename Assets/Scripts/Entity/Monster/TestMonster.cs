using Attack;
using Item;
using UnityEditor;
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
        var Obj = GameObject.Instantiate(prefab, gameObject.transform);

        ItemData? data = DataManager.Instance.FindItemData(1);
        if (data is ItemData Iteminfo)
        {
             Obj.GetComponent<OrbitingWeapon>().Initalize(Iteminfo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
