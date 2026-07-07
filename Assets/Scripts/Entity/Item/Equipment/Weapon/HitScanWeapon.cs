using Item;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HitScanWeapon : WeaponBase
{
    [SerializeField] private GameObject     BulletPrfab;
    private List<HitScanBullet>             BulletList = new List<HitScanBullet>();

    void OnEnable()
    {
        StartCoroutine(RepeatActionCoroutine());
    }

    IEnumerator RepeatActionCoroutine()
    {
        if (level <= 0) yield return null;

        while (true)
        {
            var Config = WeaponData.WeaponConfigs[level - 1];

            foreach(var Bullet in BulletList)
                Bullet.gameObject.SetActive(true);

            yield return new WaitForSeconds(Config.fInterval);
        }
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
    }

    protected override bool bIsUseItem()
    {
        return true;
    }

    protected override void UseItemLogic()
    {

    }

    protected override void SettingLevelData()
    {
        int Count = BulletList.Count;
        var Config = WeaponData.WeaponConfigs[level - 1];
        int NeedCnt = Config.iMaxLineCount - Count;

        for(int i = 0; i < NeedCnt; i++)
            BulletList.Add(GameObject.Instantiate(BulletPrfab, gameObject.transform).GetComponent<HitScanBullet>());

        float angle = 360 / BulletList.Count;
        for (int i = 0; i < BulletList.Count; i++)
        {
            Vector2 dir = Quaternion.Euler(0, 0, angle * i) * Vector2.right;
            BulletList[i].Initialize(Config.iCount, Config.fRange, dir, spriteTexs);
        }
    }
}
