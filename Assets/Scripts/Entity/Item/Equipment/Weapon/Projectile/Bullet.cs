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

            Vector2 CamPos = Camera.main.transform.position;
            float Distance = ((Vector3)CamPos - transform.position).magnitude;
            if (Distance >= 10f)
                Release();
        }
    }

    public void Initalize()
    {

    }

    public override void ShootProjectile(Projectileinfo projectileinfo, Vector2 vdir, Sprite Tex)
    {
        base.ShootProjectile(projectileinfo, vdir, Tex);

        // 로컬 x, y를 사용하면 캐릭터의 로컬 right 기준 각도가 나옵니다.
        float degAngle = Mathf.Atan2(vDir.y, vDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, degAngle));

        spriteRenderer.sprite = Tex;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Enter : {other.name}");

        var Monster = other.gameObject.GetComponent<Monster>();
        if(Monster != null)
        {
            Monster.Damaged(Monster.gameObject, new SAttackData(info.fDamage, 1, EAttackType.Strike));
            Release();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exit : {other.name}");
    }
}
