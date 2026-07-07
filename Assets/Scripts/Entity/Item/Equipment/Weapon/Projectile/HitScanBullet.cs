using Item;
using Attack;
using System.Collections;
using UnityEngine;

public class HitScanBullet : ProjectileBase
{
    private int     _AttackCnt = 0;
    private float   AccTime = 0;

    private float fShowTime = 1f;
    private float iTime = 0;

    private Sprite[] spriteTexs;
    private CircleCollider2D circleCollider = null;

    public void Initialize(int AttackCnt,
                           float Range,       
                           Vector3 vdir,
                           Sprite[] sprites)
    {
        if (spriteRenderer == null)
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if (circleCollider == null)
            circleCollider = GetComponent<CircleCollider2D>();

        spriteTexs = sprites;

        _AttackCnt = AttackCnt;
        vDir = vdir.normalized;
        circleCollider.radius = Range;
        iTime = fShowTime / spriteTexs.Length;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        StartCoroutine(RepeatActionCoroutine());
    }

    // Update is called once per frame
    private void Update()
    {
        AccTime += Time.deltaTime;
        if (AccTime >= fShowTime)
            AccTime = 0;
        else
        {
            int iIndex = (int)(AccTime / iTime);
            spriteRenderer.sprite = spriteTexs[iIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var Mon = collision.gameObject.GetComponent<Monster>();
        if (Mon == null)
            return;

        Mon.Damaged(gameObject, new Attack.SAttackData(20));
        Debug.Log($"Hit BoundBall : {Mon.name}");
    }

    IEnumerator RepeatActionCoroutine()
    {
        int ATKcnt = 0;
        float interval = fShowTime / _AttackCnt;

        if (circleCollider == null) 
            yield return null;

        while (ATKcnt < _AttackCnt)
        {
            Attack(ATKcnt);
            ATKcnt++;

            yield return new WaitForSeconds(interval);
        }

        circleCollider.isTrigger = false;
        gameObject.SetActive(false);
        yield return null;
    }

    void Attack(int AtkCnt)
    {
        var PlayerPos = InGameManager.Instance.GetPlayerTransform().position;
        transform.position = PlayerPos + vDir * (AtkCnt + 1);
        circleCollider.isTrigger = true;
    }
}
