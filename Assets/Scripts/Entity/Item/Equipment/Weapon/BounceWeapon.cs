using Item;
using System;
using UnityEngine;

public class BounceWeapon : WeaponBase
{
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
        Vector3 camPos = cam.transform.position;
        camPos.z = 0f;
        transform.position = camPos + (Vector3)localPos;
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        SettingLevelData();

        cam = Camera.main;

        radius = gameObject.transform.localScale.x * 0.2f;
        localPos = Vector2.zero;
        dir = UnityEngine.Random.insideUnitCircle.normalized;
        transform.position = InGameManager.Instance.GetPlayerTransform().position;
    }

    void ComputeScreenSize()
    {
        // 화면 크기 계산
        bool bIsReflect = false;
        BoundSize.x = cam.orthographicSize - radius;
        BoundSize.y = BoundSize.x * cam.aspect - radius;

        // X축 반사
        if (localPos.x < -BoundSize.y)
        {
            localPos.x = -BoundSize.y;
            dir.x = Mathf.Abs(dir.x);
            bIsReflect = true;
        }
        else if (localPos.x > BoundSize.y)
        {
            localPos.x = BoundSize.y;
            dir.x = -Mathf.Abs(dir.x);
            bIsReflect = true;
        }

        // Y축 반사
        if (localPos.y < -BoundSize.x)
        {
            localPos.y = -BoundSize.x;
            dir.y = Mathf.Abs(dir.y);
            bIsReflect = true;
        }
        else if (localPos.y > BoundSize.x)
        {
            localPos.y = BoundSize.x;
            dir.y = -Mathf.Abs(dir.y);
            bIsReflect = true;
        }

        if(bIsReflect)
        {
            foreach(var particleID in WeaponData.LevelDatas[level - 1].ParticleIDs)
            {
                var EffectSO = DataManager.Instance.FindEffectSO(particleID);
                var obj = ObjectPoolManager.Instance.Get(EffectSO);

                obj.gameObject.transform.position = transform.position;
            }
          
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
}
