using Item;
using System;
using UnityEngine;


public class BounceWeapon : WeaponBase
{
    private static readonly Vector2 BoundOffset = new Vector2(2.5f, 2.5f);

    private float   radius = 0.3f;
    private Vector2 localPos;
    private Vector2 dir;

    private Camera  cam;
    private Vector2 BoundSize;

    // Update is called once per frame
    void Update()
    {
        if (cam == null)
            return;

        // 플레이어 기준 좌표 이동
        localPos += dir * WeaponData.WeaponConfigs[level - 1].fSpeed * Time.deltaTime;
        ComputeScreenSize();
        transform.position = (Vector3)localPos;
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        SettingLevelData();

        cam = Camera.main;

        radius = gameObject.transform.localScale.x;
        localPos = InGameManager.Instance.GetPlayerTransform().position;
        dir = UnityEngine.Random.insideUnitCircle.normalized;
        transform.position = InGameManager.Instance.GetPlayerTransform().position;
    }

    void ComputeScreenSize()
    {
        // 화면 크기 계산
        bool bIsReflect = false;
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1));

        float left = min.x - BoundOffset.x + radius;
        float right = max.x + BoundOffset.x - radius;
        float bottom = min.y - BoundOffset.y + radius;
        float top = max.y + BoundOffset.y - radius;

        // X
        if (localPos.x < left)
        {
            localPos.x = left;
            dir.x = Mathf.Abs(dir.x);
            bIsReflect = true;
        }
        else if (localPos.x > right)
        {
            localPos.x = right;
            dir.x = -Mathf.Abs(dir.x);
            bIsReflect = true;
        }

        // Y
        if (localPos.y < bottom)
        {
            localPos.y = bottom;
            dir.y = Mathf.Abs(dir.y);
            bIsReflect = true;
        }
        else if (localPos.y > top)
        {
            localPos.y = top;
            dir.y = -Mathf.Abs(dir.y);
            bIsReflect = true;
        }

        if (bIsReflect)
        {
            float angle = UnityEngine.Random.Range(-10f, 10f);
            dir = Quaternion.AngleAxis(angle, Vector3.forward) * dir;

            /*foreach (var particleID in WeaponData.LevelDatas[level - 1].ParticleIDs)
            {
                var EffectSO = DataManager.Instance.FindEffectSO(particleID);
                var obj = ObjectPoolManager.Instance.Get(EffectSO);

                obj.gameObject.transform.position = transform.position;
            }*/
        }
    }   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var Mon = collision.gameObject.GetComponent<Monster>();
        if (Mon == null)
            return;

        Mon.Damaged(gameObject, new Attack.SAttackData(20));
    }
}
