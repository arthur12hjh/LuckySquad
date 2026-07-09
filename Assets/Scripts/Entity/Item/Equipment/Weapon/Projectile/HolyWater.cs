using Attack;
using DG.Tweening;
using Item;
using UnityEngine;

public class HolyWater : ProjectileBase
{
    [SerializeField] LayerMask LayerMask;

    Animator   animator = null;
    Vector3     TargetPoint = Vector3.zero;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if(animator == null)
            animator = GetComponent<Animator>();

        animator.speed = 1f;
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void DamagedAct()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.3f, LayerMask.value);
        foreach (var hit in hits)
        {
            hit.GetComponent<Monster>()?.Damaged(gameObject, new SAttackData(20));
        }
    }

    public override void ShootProjectile(Projectileinfo projectileinfo, Vector2 vdir, string AtalsName)
    {
        base.ShootProjectile(projectileinfo, vdir, AtalsName);

        if (SpriteTexs.Length > projectileinfo.iLevel)
            spriteRenderer.sprite = SpriteTexs[0];

        transform.DOMove(TargetPoint, 1)
            .OnComplete(() =>
            {
                animator.speed = 1f;
                animator.Play("Base", 0, 0f);
            });
    }
}
