using Item;
using Attack;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class HitScanBullet : ProjectileBase
{
    private int     _AttackCnt = 0;
    private float   AccTime = 0;

    private float fShowTime = 1f;
    private float iTime = 0;

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

        SpriteTexs = sprites;

        _AttackCnt = AttackCnt;
        vDir = vdir.normalized;
        circleCollider.radius = Range;
        iTime = fShowTime / SpriteTexs.Length;
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
            spriteRenderer.sprite = SpriteTexs[iIndex];
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
        var PlayerPos = InGameManager.Instance.GetPlayerTransform().position;

        if (circleCollider == null) 
            yield return null;

        while (ATKcnt < _AttackCnt)
        {
            Attack(PlayerPos, ATKcnt);
            ATKcnt++;

            yield return new WaitForSeconds(interval);
        }

        circleCollider.isTrigger = false;
        gameObject.SetActive(false);
        yield return null;
    }

    void Attack(Vector3 vPos, int AtkCnt)
    {
        float AngleX = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad; ;
        float AngleY = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad; ;

        vDir = new Vector3( Mathf.Cos(AngleY), Mathf.Sin(AngleX));
        transform.position = vPos + vDir * (AtkCnt + 1);
        circleCollider.isTrigger = true;
    }
}
