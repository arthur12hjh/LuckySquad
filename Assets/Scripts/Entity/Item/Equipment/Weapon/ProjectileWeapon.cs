using Item;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class ProjectileWeapon : EquipmentBase
{
    public enum ProjectileType { Projectile, Throw, None};

    [SerializeField] private ObjectPoolRef  projectTileRefSO = null;
    [SerializeField] private int            LineAngle = 90;
    [SerializeField] private string         BulletTextureUrl;
    [SerializeField] private ProjectileType type = ProjectileType.Projectile;

    private SpawnPattern        spawnPattern = null;
    private Sprite[]            BulletSpriteTex;
    private ProjectTileEffect   projectTileEffect;

    private Vector2             vDir = Vector2.zero;
    private float               fSpeed = 3f;
    private int                 iShootCount = 0;

    // Start i s called before the first frame update
    void Start()
    {
       
    }

    public override void Update_Directation(Vector2 dir)
    {
        if (dir == Vector2.zero)
            return;

        vDir = dir;
    }

    public override void Initalize(ItemData itemData)
    {
        base.Initalize(itemData);
        var sprite = AddressablesManager.Instance.GetLabelObject<SpriteAtlas>("Lobby", "imgAtlas", BulletTextureUrl);
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

        SerializationWeaponData();
        StartCoroutine(RepeatActionCoroutine());
       
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
       if (ItemData.iID == 0 || spawnPattern == null)
            return;

        if (vDir == Vector2.zero && type == ProjectileType.Projectile)
            return;

        int ShootLineCnt = GetShootLineCount();
        int angle = LineAngle / ShootLineCnt;
        int startAngle = -(angle * (ShootLineCnt - 1)) / 2; ;

        for (int i = 0, AccAngle = startAngle; i < projectTileEffect.ilineCount; i++, AccAngle += angle)
        {
            var gameObj = ObjectPoolManager.Instance.Get(projectTileRefSO);
            Vector3 newDir = Quaternion.Euler(0, 0, AccAngle) * vDir;

            gameObj.SetActive(true);
            gameObj.transform.position = spawnPattern.GetPosition(Vector3.zero);
            gameObj.GetComponent<ProjectileBase>().ShootProjectile(new Projectileinfo(fSpeed, projectTileEffect.fDamage), newDir, BulletSpriteTex[level - 1]);
        }

        iShootCount++;
    }

    int GetShootLineCount()
    {
        return projectTileEffect.ilineCount;
    }
}
