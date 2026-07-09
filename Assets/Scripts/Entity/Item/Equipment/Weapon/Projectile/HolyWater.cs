using Attack;
using DG.Tweening;
using Item;
using UnityEngine;
using static EfffectRef;

public class HolyWater : ProjectileBase
{
    [SerializeField] LayerMask LayerMask;


    Vector3     TargetPoint = Vector3.zero;

    private void OnEnable()
    { 
        if(animator  == null)
            animator = GetComponent<Animator>();

        animator.speed = 0f;
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

        foreach (var particleID in _levelData.ParticleIDs)
        {
            var refSO = DataManager.Instance.FindEffectSO(particleID);
            var obj = ObjectPoolManager.Instance.Get(refSO);

            var efffectRef = refSO as EfffectRef;
            obj.gameObject.transform.position = transform.position;
            obj.SetActive(true);
            if (efffectRef._Type == EEffectSOType.DamageEffect)
            {
                if (_levelData.ItemEffects.TryGetValue(EEffectType.Damage, out var list))
                {
                    var FogEft = obj.gameObject.GetComponent<GroundFogEffect>();
                    if(FogEft != null)
                    {
                        FogEft.Initailize(info, list[0] as DamageEffect);
                    }
                }
            }
         }
    }

    protected override void ShootProjectile(Projectileinfo projectileinfo,
                                         Vector2 vdir,
                                         string AtalsName,
                                         string ControolerName)
    {
        base.ShootProjectile(projectileinfo, vdir, AtalsName, ControolerName);

        if (SpriteTexs.Length > projectileinfo.iLevel)
            spriteRenderer.sprite = SpriteTexs[0];

        TargetPoint = UtilitySystem.GetRandomWorldPoint(Camera.main);

        transform.DOMove(TargetPoint, 1)
            .OnComplete(() =>
            {
                animator.speed = 1f;
                animator.Play("Base", 0, 0f);
            });
    }
}
