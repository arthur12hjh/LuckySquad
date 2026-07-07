using Item;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class ProjectileWeapon : WeaponBase
{
    public enum ProjectileType { Projectile, Throw, None};

    [SerializeField] private ObjectPoolRef  projectTileRefSO = null;
    [SerializeField] private int            LineAngle = 90;
    [SerializeField] private string         BulletTextureUrl;
    [SerializeField] private float          fSpeed = 3f;
    [SerializeField] private ProjectileType type = ProjectileType.Projectile;

    private SpawnPattern        spawnPattern = null;
    private Sprite[]            BulletSpriteTex;

    private int                 iShootCount = 0;

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        var sprite = AddressablesManager.Instance.GetCommon<SpriteAtlas>(BulletTextureUrl);
        if (sprite != null)
        {
            BulletSpriteTex = new Sprite[sprite.spriteCount];
            sprite.GetSprites(BulletSpriteTex);
        }

        switch (type)
        {
            case ProjectileType.Projectile:
                spawnPattern = ProjectileSpawnPattern.Create(gameObject.transform.parent.gameObject);
                break;

            case ProjectileType.Throw:
                spawnPattern = InGameManager.Instance.OutScreenSpawnPattern;
                break;
        }

        SettingLevelData();
        IsActive = true;
    }

    private void OnEnable()
    {
        StartCoroutine(RepeatActionCoroutine());
    }

    IEnumerator RepeatActionCoroutine()
    {
        if (level <= 0) yield return null;

        while (true)
        {
            ShootBulletEvent();

            float interval = 0.2f;
            if (iShootCount >= WeaponData.WeaponConfigs[level - 1].iCount)
            {
                iShootCount = 0;
                interval = WeaponData.WeaponConfigs[level - 1].fInterval;
            }

            yield return new WaitForSeconds(interval);
        }
    }

    private void ShootBulletEvent()
    {
       if (ItemData.iID == 0 || spawnPattern == null)
            return;

        Vector2 vDir = InGameManager.Instance.GetPlayerDir();
        if (vDir == Vector2.zero && type == ProjectileType.Projectile)
            return;

        int ShootLineCnt = GetShootLineCount();
        int angle = LineAngle / ShootLineCnt;
        int startAngle = -(angle * (ShootLineCnt - 1)) / 2; ;

        int LineCount = GetShootLineCount();
        for (int i = 0, AccAngle = startAngle; i < LineCount; i++, AccAngle += angle)
        {
            if (ObjectPoolManager.Instance == null)
                break;

            var gameObj = ObjectPoolManager.Instance.Get(projectTileRefSO);
            Vector3 newDir = Quaternion.Euler(0, 0, AccAngle) * vDir;

            gameObj.SetActive(true);
            gameObj.transform.position = spawnPattern.GetPosition(Vector3.zero);
            gameObj.GetComponent<ProjectileBase>().ShootProjectile(new Projectileinfo(
                                       fSpeed, 
                                       WeaponData.WeaponConfigs[level - 1].fDamage),
                                       newDir, 
                                       BulletSpriteTex[level - 1]);
        }

        iShootCount++;
    }

    int GetShootLineCount()
    {
        return WeaponData.WeaponConfigs[level - 1].iMaxLineCount;
    }
}
