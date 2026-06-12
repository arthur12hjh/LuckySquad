using Item;
using System.Collections;
using System.Linq;
using UnityEngine;

public class HitScanWeapon : EquipmentBase
{
    private DamageEffect        damageEffect;
    private int                 iAttackCnt = 0;
    private float               AccTime = 0;

    private float               fShowTime = 0.3f;
    private float               iTime = 0;

    private CircleCollider2D    circleCollider = null;

    void Start()
    {
        StartCoroutine(RepeatActionCoroutine());
    }

    private void Update()
    {
        if (circleCollider == null)
            return;

        if (circleCollider.isTrigger)
        {
            AccTime += Time.deltaTime;
            if (AccTime >= fShowTime)
            {
                circleCollider.isTrigger = false;
                spriteRenderer.sprite = null;
                AccTime = 0;
            }
            else
            {
                int iIndex = (int)(AccTime / iTime);
                spriteRenderer.sprite = spriteTexs[iIndex];
            }
        }
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);

        circleCollider = GetComponent<CircleCollider2D>();
        iTime = fShowTime / spriteTexs.Count();

        SerializationWeaponData();
    }

    protected override bool bIsUseItem()
    {
        return true;
    }

    protected override bool bIsLevelUpItem()
    {
        return true;
    }

    protected override void UseItemLogic()
    {

    }

    protected override void SettingLevelData()
    {
        SerializationWeaponData();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exit : {other.name}");
    }

    private bool SerializationWeaponData()
    {
        if (info.LevelDatas.Count < level)
            return false;

        
        foreach (var effect in info.LevelDatas[level - 1].Effects)
        {
            if (effect is DamageEffect infoProjectileEffect)
            {
                damageEffect = infoProjectileEffect;
            }
        }

        return true;
    }

    IEnumerator RepeatActionCoroutine()
    {
        while (true)
        {
            float interval = 0.3f;
            int ATK_cnt = GetWeaponAttackCount();

            Attack();
            if (iAttackCnt >= ATK_cnt)
            {
                iAttackCnt = 0;
                interval = damageEffect.fInterval;
            }

            yield return new WaitForSeconds(interval);
        }
    }

    void Attack()
    {
        transform.position = Vector3.zero + Vector3.one * iAttackCnt;

        circleCollider.radius = damageEffect.fRange;
        circleCollider.isTrigger = true;
        iAttackCnt++;
    }

    int GetWeaponAttackCount()
    {
        return damageEffect.iCount + 2;
    }
}
