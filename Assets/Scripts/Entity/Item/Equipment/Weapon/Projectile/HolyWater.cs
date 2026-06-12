using Attack;
using Item;
using UnityEngine;

public class HolyWater : ProjectileBase
{
    [SerializeField] Sprite TempTex = null;
    Collider2D  collider2D = null;
    
    Vector3     StartPoint = Vector3.zero;
    Vector3     TargetPoint = Vector3.zero;

    float       AccTime = 0f;
    bool        AttackAble = false;

    void Awake()
    {
        collider2D = GetComponent<CapsuleCollider2D>();
        collider2D.isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsAlive)
        {
            AccTime += Time.deltaTime;
            if (AttackAble)
            {
                if (AccTime >= 50f)
                {
                    collider2D.isTrigger = false;
                    bIsAlive = false;
                    Release();
                }
            }
            else
            {
                if (AccTime < 10f)
                {
                    transform.position = Vector3.Lerp(StartPoint, TargetPoint, AccTime / 10f);
                }
                else
                {
                    AccTime = 0;
                    spriteRenderer.sprite = TempTex;
                    gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0f);
                    collider2D.isTrigger = true;
                    AttackAble = true;
                }
            }
        }
    }

    public override void ShootProjectile(Projectileinfo projectileinfo, Vector2 vdir, Sprite Tex)
    {
        base.ShootProjectile(projectileinfo, vdir, Tex);

        AccTime = 0;
        AttackAble = false;
        StartPoint = transform.position;
    }
}
