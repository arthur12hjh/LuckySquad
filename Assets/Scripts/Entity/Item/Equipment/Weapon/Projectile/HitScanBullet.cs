using Item;
using Attack;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class HitScanBullet : ProjectileBase
{
    [SerializeField] LayerMask LayerMask;

    private int     _AttackCnt = 0;
    Animator        _animator = null;
    float           _Range = 0.3f;

    public void Initialize(int AttackCnt,
                           float Range,       
                           Vector3 vdir,
                           Sprite[] sprites)
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        _AttackCnt = AttackCnt;
        vDir = vdir.normalized;
        _Range = Range;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        StartCoroutine(RepeatActionCoroutine());
    }

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.3f, LayerMask.value);
        foreach (var hit in hits)
        {
            hit.GetComponent<Monster>()?.Damaged(gameObject, new SAttackData(20));
        }
    }

    IEnumerator RepeatActionCoroutine()
    {
        int ATKcnt = 0;
        var PlayerPos = InGameManager.Instance.GetPlayerTransform().position;
        if (_animator == null)
            yield return null;

        while (ATKcnt < _AttackCnt)
        {
            Attack(PlayerPos, ATKcnt);
            ATKcnt++;

            yield return new WaitForSeconds(0.7f);
        }

        gameObject.SetActive(false);
        yield return null;
    }

    void Attack(Vector3 vPos, int AtkCnt)
    {
        float AngleX = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad; ;
        float AngleY = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad; ;

        vDir = new Vector3( Mathf.Cos(AngleY), Mathf.Sin(AngleX));

        _animator.speed = 1f;
        _animator.Play("Base", 0, 0);
        transform.position = vPos + vDir * (AtkCnt + 1);
    }

    public override void Release()
    {
        _animator.speed = 0f;
        base.Release();
    }
}
