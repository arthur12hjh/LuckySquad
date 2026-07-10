using DG.Tweening;
using Item;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class ProjectileWeapon : WeaponBase
{
    public enum ProjectileType { Projectile, Throw, None};

    [SerializeField] private ObjectPoolRef  projectTileRefSO = null;
    [SerializeField] private int            LineAngle = 90;
    [SerializeField] private float          fSpeed = 10f;
    [SerializeField] private ProjectileType type = ProjectileType.Projectile;

    private SpawnPattern        spawnPattern = null;
    private int                 iShootCount = 0;

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        
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

    private void Update()
    {
        Vector3  vPlayerDir = InGameManager.Instance.GetPlayerDir();
        float angle = Mathf.Atan2(vPlayerDir.y, vPlayerDir.x) * Mathf.Rad2Deg;
        if (vPlayerDir.x < 0)
        {
            spriteRenderer.flipX = true;
            angle += 180f; // 또는 -angle, 축 설정에 따라 다름
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        transform.DORotate(new Vector3(0, 0, angle), 0.3f);
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
        var weaponConfig = WeaponData.WeaponConfigs[level - 1];

        for (int i = 0, AccAngle = startAngle; i < LineCount; i++, AccAngle += angle)
        {
            if (ObjectPoolManager.Instance == null)
                break;

            var gameObj = ObjectPoolManager.Instance.Get(projectTileRefSO);
            if (gameObj == null)
                return;

            Vector3 newDir = Quaternion.Euler(0, 0, AccAngle) * vDir;

            gameObj.SetActive(true);
            gameObj.transform.position = spawnPattern.GetPosition(Vector3.zero);

            gameObj.GetComponent<ProjectileBase>().Initalize(
                                       WeaponData.LevelDatas[level - 1],
                                       new Projectileinfo(level, weaponConfig),
                                       newDir,
                                       WeaponData.BulletName, WeaponData.AnimController);
        }

        iShootCount++;
    }

    int GetShootLineCount()
    {
        return WeaponData.WeaponConfigs[level - 1].iMaxLineCount;
    }
}
