using Item;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingWeapon : WeaponBase
{
    [SerializeField] private ObjectPoolRef projectTileRefSO = null;
    List<GameObject> ProjecTileList = new List<GameObject>();

    private float               TickAngle = 0f;
    private float               fSpeed = 3f;
    private int                 iActiveProjectile = 0;

    void Update()
    {
        if (IsActive)
        {
           transform.Rotate(0f,0f, 180f *  Time.deltaTime * fSpeed);
        }
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        SerializationWeaponData();

        transform.localPosition = new Vector3(0f, transform.parent.transform.localScale.y * 0.5f, 0f);
        IsActive = true;
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
        SerializationWeaponData();
    }

    private bool SerializationWeaponData()
    {
        if (info.MaxLevel < level)
            return false;

        TickAngle = 360f / WeaponData.WeaponConfigs[level - 1].iCount;
        iActiveProjectile = GetProjectileCount();

        CreateProjectile();
        ComputeProjecTilePosition();

        return true;
    }

    private void CreateProjectile()
    {
        if (iActiveProjectile - ProjecTileList.Count > 0)
        {
            for (int i = iActiveProjectile - ProjecTileList.Count; i > 0; --i)
            {
                var gameOb = ObjectPoolManager.Instance.Get(projectTileRefSO);
                gameOb.transform.parent = gameObject.transform;
                gameOb.SetActive(false);
                var ObjSR  = gameOb.GetComponent<SpriteRenderer>();
                if(ObjSR != null)
                    ObjSR.sprite = spriteTexs[level - 1];

                ProjecTileList.Add(gameOb);
            }
        }
    }

    private void ComputeProjecTilePosition()
    {
        float range = WeaponData.WeaponConfigs[level - 1].fRange;
        int Count = WeaponData.WeaponConfigs[level - 1].iCount;

        for (int i = 0; i < ProjecTileList.Count; ++i)
        {
            if (i < Count)
            {
                float rad = i * TickAngle * Mathf.Deg2Rad;
                
                float posX = Mathf.Sin(rad) * range;
                float posY = Mathf.Cos(rad) * range;

                ProjecTileList[i].transform.localPosition = new Vector3(posX, posY, 0);
                ProjecTileList[i].SetActive(true);
            }
            else
                ProjecTileList[i].SetActive(false);
        }
    }

    int GetProjectileCount()
    {
        return WeaponData.WeaponConfigs[level - 1].iCount;
    }

    private void OnDestroy()
    {
        foreach (var obj in ProjecTileList)
        {
            obj.GetComponent<ProjectileBase>().Release();
        }
    }
}
