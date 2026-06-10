using Item;
using System.Collections;
using UnityEngine;
public class BounceWeapon : EquipmentBase
{
    private ProjectTileEffect projectTileEffect;
    private Rigidbody2D       BounceRb = null;

    Vector3 vDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BounceRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            int wallMask = 1 << 6;
            float moveDist = projectTileEffect.fSpeed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, vDir, moveDist, wallMask);
            if (hit.collider != null)
            {
                vDir = Vector2.Reflect(vDir, hit.normal);
            }
            else
            {
                transform.position += vDir * moveDist;
            }
        }
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        SerializationWeaponData();

        gameObject.transform.position = gameObject.transform.parent.position;
        vDir = Random.insideUnitCircle.normalized;
        IsActive = true;
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
