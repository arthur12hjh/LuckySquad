using Attack;
using Item;
using UnityEngine;
using UnityEngine.UIElements;

public class Orbiting : ProjectileBase
{
    public void Initalize()
    {

    }

    protected override void ShootProjectile(Projectileinfo projectileinfo,
                                     Vector2 vdir,
                                     string AtalsName,
                                     string ControolerName)
    {
        base.ShootProjectile(projectileinfo, vdir, AtalsName, ControolerName);
        Vector2 dir = transform.localPosition.normalized;

        transform.localRotation = Quaternion.FromToRotation(Vector3.right, dir);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Enter : {other.name}");

        var Monster = other.gameObject.GetComponent<Monster>();
        if(Monster != null)
        {
            Monster.Damaged(Monster.gameObject, new SAttackData(info._config.fDamage, 1, EAttackType.Strike));
        }
    }
}
