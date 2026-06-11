using Item;
using System.Collections;
using UnityEngine;

public class ProjectileWeapon : EquipmentBase
{
    [SerializeField] private ObjectPoolRef projectTileRefSO = null;
    [SerializeField] private int           LineAngle = 90;
    [SerializeField] private string        BulletTextureUrl;

    private Sprite[]            BulletSpriteTex;
    private ProjectTileEffect   projectTileEffect;
    private Vector2             vDir = Vector2.zero;
    private float               fSpeed = 3f;
    private int                 iShootCount = 0;

    // Start i s called before the first frame update
    void Start()
    {
        StartCoroutine(RepeatActionCoroutine());
    }

    public void Update_Directation(Vector2 dir)
    {
        vDir = dir;
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        BulletSpriteTex = Resources.LoadAll<Sprite>(BulletTextureUrl);

        level++;
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
            if(effect is ProjectTileEffect infoProjectileEffect)
            {
                projectTileEffect = infoProjectileEffect;
            }
        }

        return true;
    }

    IEnumerator RepeatActionCoroutine()
    {
        while (true)
        {
            ShootBulletEvent();

            float interval = 0.2f;
            if (iShootCount >= projectTileEffect.iCount)
            {
                iShootCount = 0;
                interval = projectTileEffect.fInterval;
            }

            yield return new WaitForSeconds(interval);
        }
    }

    private void ShootBulletEvent()
    {
       if (vDir == Vector2.zero || ItemData.iID == 0)
            return;

        int ShootLineCnt = GetShootLineCount();
        int angle = LineAngle / ShootLineCnt;
        int startAngle = -(angle * (ShootLineCnt - 1)) / 2; ;

        for (int i = 0, AccAngle = startAngle; i < projectTileEffect.ilineCount; i++, AccAngle += angle)
        {
            var gameObj = ObjectPoolManager.Instance.Get(projectTileRefSO);
            Vector3 newDir = Quaternion.Euler(0, 0, AccAngle) * vDir;

            gameObj.SetActive(true);
            gameObj.transform.position = transform.position;
            gameObj.GetComponent<Projectile>().ShootProjectile(new Projectileinfo(fSpeed, projectTileEffect.fDamage), newDir, BulletSpriteTex[level - 1]);
        }

        iShootCount++;
    }

    int GetShootLineCount()
    {
        return projectTileEffect.ilineCount;
    }
}
