using Item;
using System;
using UnityEngine;

public class BounceWeapon : EquipmentBase
{
    private ProjectTileEffect           projectTileEffect;
    
    private float   radius = 0.3f;
    private Vector2 localPos;
    private Vector2 dir;

    private Camera  cam;
    private Vector2 BoundSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       

       
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null)
            return;

        // 플레이어 기준 좌표 이동
        localPos += dir * projectTileEffect.fSpeed * Time.deltaTime;
        ComputeScreenSize();
        Vector3 camPos = cam.transform.position;
        camPos.z = 0f;
        transform.position = camPos + (Vector3)localPos;
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        SerializationWeaponData();

        cam = Camera.main;

        radius = gameObject.transform.localScale.x * 0.2f;
        localPos = Vector2.zero;
        dir = UnityEngine.Random.insideUnitCircle.normalized;
        transform.position = InGameManager.Instance.GetPlayerTransform().position;
    }

    void ComputeScreenSize()
    {
        // 화면 크기 계산
        BoundSize.x = cam.orthographicSize - radius;
        BoundSize.y = BoundSize.x * cam.aspect - radius;

        // X축 반사
        if (localPos.x < -BoundSize.y)
        {
            localPos.x = -BoundSize.y;
            dir.x = Mathf.Abs(dir.x);
        }
        else if (localPos.x > BoundSize.y)
        {
            localPos.x = BoundSize.y;
            dir.x = -Mathf.Abs(dir.x);
        }

        // Y축 반사
        if (localPos.y < -BoundSize.x)
        {
            localPos.y = -BoundSize.x;
            dir.y = Mathf.Abs(dir.y);
        }
        else if (localPos.y > BoundSize.x)
        {
            localPos.y = BoundSize.x;
            dir.y = -Mathf.Abs(dir.y);
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var Mon = collision.gameObject.GetComponent<Monster>();
        if (Mon == null)
            return;

        Mon.Damaged(gameObject, new Attack.SAttackData(20));
        Debug.Log($"Hit BoundBall : {Mon.name}");
    }


    private bool SerializationWeaponData()
    {
        if (info.LevelDatas.Count < level)
            return false;

        spriteRenderer.sprite = spriteTexs[level - 1];
        foreach (var effect in info.LevelDatas[level - 1].Effects)
        {
            if (effect is ProjectTileEffect infoProjectileEffect)
            {
                projectTileEffect = infoProjectileEffect;
            }
        }

        return true;
    }
}
