using Item;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingWeapon : EquipmentBase
{
    [SerializeField] private ObjectPoolRef projectTileRefSO = null;
    List<GameObject> ProjecTileList = new List<GameObject>();

    private ProjectTileEffect   projectTileEffect;
    private float               TickAngle = 0f;
    private float               fSpeed = 3f;
    private int                 iActiveProjectile = 0;

    void Start()
    {

    }

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

        
        foreach (var effect in info.LevelDatas[level - 1].Effects)
        {
            if (effect is ProjectTileEffect infoProjectileEffect)
            {
                projectTileEffect = infoProjectileEffect;

                TickAngle = 360f / projectTileEffect.iCount;
                iActiveProjectile = GetProjectileCount();

                CreateProjectile();
                ComputeProjecTilePosition();
            }
        }

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
        for (int i = 0; i < ProjecTileList.Count; ++i)
        {
            if (i < projectTileEffect.iCount)
            {
                float rad = i * TickAngle * Mathf.Deg2Rad;
                float posX = Mathf.Sin(rad) * projectTileEffect.fRange;
                float posY = Mathf.Cos(rad) * projectTileEffect.fRange;

                ProjecTileList[i].transform.localPosition = new Vector3(posX, posY, 0);
                ProjecTileList[i].SetActive(true);
            }
            else
                ProjecTileList[i].SetActive(false);
        }
    }

    int GetProjectileCount()
    {
        return projectTileEffect.iCount;
    }

    private void OnDestroy()
    {
        foreach (var obj in ProjecTileList)
        {
            obj.GetComponent<ProjectileBase>().Release();
        }
    }
}
