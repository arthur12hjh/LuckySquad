using Item;
using Attack;
using UnityEngine;

public class Bullet : ProjectileBase
{
    // Update is called once per frame
    void Update()
    {
        if(bIsAlive)
        {
            transform.position += vDir * info.fSpeed * Time.deltaTime;
        }
    }

    public void Initalize()
    {

    }

    public override void ShootProjectile(Projectileinfo projectileinfo, Vector2 vdir, Sprite Tex)
    {
        info = projectileinfo;
        vDir = vdir;
        bIsAlive = true;

        if (spriteRenderer == null)
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = Tex;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Enter : {other.name}");

        var Monster = other.gameObject.GetComponent<TestMonster>();
        if(Monster != null)
        {
            Monster.Dagmed(Monster.gameObject, new SAttackData(info.fDamage, 1, EAttackType.Strike));
            Release();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exit : {other.name}");
    }
}
