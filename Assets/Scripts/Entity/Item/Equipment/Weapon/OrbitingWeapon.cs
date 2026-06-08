using Item;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingWeapon : EquipmentBase
{
    [SerializeField] private GameObject projectTile = null;
    List<GameObject> CircleList = new List<GameObject>();
 
    private float    TickAngle = 0f;
    private float    Speed = 3f;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            LevelUp();
        }

        if (IsActive)
        {
            transform.Rotate(0, 0, 180 * Time.deltaTime * Speed);
        }
    }

    public override void Initalize(ItemData Data)
    {
        base.Initalize(Data);
        Create_Projectile();
        SettingProjectile();

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
        
        SettingProjectile();
    }

    private bool Create_Projectile()
    {
        if(info.LevelDatas[info.MaxLevel - 1] != null)
        {
            foreach (var Effect in info.LevelDatas[info.MaxLevel - 1].Effects)
            {
                if (Effect is ProjectTileEffect effect)
                {
                    for (int i = 0; i < effect.iCount; ++i)
                    {
                        var gameOb = GameObject.Instantiate(projectTile, gameObject.transform);

                        gameOb.SetActive(false);
                        CircleList.Add(gameOb);
                    }
                }
            }
        }
        else
            return false;

        return true;
    }

    private bool SettingProjectile()
    {
        if (info.LevelDatas[level - 1] != null)
        {
            foreach (var Effect in info.LevelDatas[level - 1].Effects)
            {
                if (Effect is ProjectTileEffect effect)
                {
                    TickAngle = 360 / effect.iCount;
                    Range = effect.fRange;

                    for (int i = 0; i < CircleList.Count; ++i)
                    {
                        if (i < effect.iCount)
                        {
                            float rad = i * TickAngle * Mathf.Deg2Rad;
                            float NewX = Mathf.Sin(rad) * Range;
                            float NewY = Mathf.Cos(rad) * Range;

                            CircleList[i].transform.position = new Vector3(NewX, NewY, 0);
                            CircleList[i].SetActive(true);
                        }
                            
                        else
                            CircleList[i].SetActive(false);
                    }
                }
            }
        }
        else
            return false;

        return true;
    }
}
