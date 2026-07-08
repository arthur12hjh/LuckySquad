using Attack;
using DG.Tweening;
using Item;
using UnityEngine;

public class HolyWater : ProjectileBase
{
    [SerializeField] Sprite TempTex = null;
    Collider2D  collider2D = null;
    
    Vector3     StartPoint = Vector3.zero;
    Vector3     TargetPoint = Vector3.zero;

    private float fShowTime = 1f;
    private float iTime = 0;

    float       AccTime = 0f;
    bool        AttackAble = false;

    void Awake()
    {
        collider2D = GetComponent<CapsuleCollider2D>();
        collider2D.isTrigger = false;
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    // Update is called once per frame
    void Update()
    {
        if (AttackAble)
        {
            AccTime += Time.deltaTime;
            if (AccTime >= fShowTime)
            {
                collider2D.isTrigger = false;
                bIsAlive = false;
                Release();
            }
            else
            {
                int iIndex = (int)(AccTime / iTime);
                spriteRenderer.sprite = SpriteTexs[iIndex];
            }
        }
    }

    public override void ShootProjectile(Projectileinfo projectileinfo, Vector2 vdir, string AtalsName)
    {
        base.ShootProjectile(projectileinfo, vdir, AtalsName);

        if (SpriteTexs.Length > projectileinfo.iLevel)
            spriteRenderer.sprite = SpriteTexs[0];

        iTime = fShowTime / SpriteTexs.Length;
        AccTime = 0;
        AttackAble = false;
        StartPoint = transform.position;
        transform.DOMove(TargetPoint, 1)
            .OnComplete(() =>
            {
                spriteRenderer.sprite = TempTex;
                gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0f);
                collider2D.isTrigger = true;
                AttackAble = true;
            });
    }
}
