using Item;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static ProjectileWeapon;

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
            transform.position += vDir * projectTileEffect.fSpeed * Time.deltaTime;
        }
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        SerializationWeaponData();

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
