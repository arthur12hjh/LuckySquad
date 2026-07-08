using Item;
using UnityEngine;
using System;
using UnityEngine.U2D;

public abstract class ProjectileBase : MonoBehaviour, IPoolable
{
    protected Action            _releaseAct;
    protected SpriteRenderer    spriteRenderer = null;
    protected Projectileinfo    info = new Projectileinfo();

    protected Sprite[]          SpriteTexs;
    protected Vector3           vDir = Vector3.zero;
    protected bool              bIsAlive = false;

    public virtual void ShootProjectile(Projectileinfo projectileinfo, Vector2 vdir, string AtalsName)
    {
        info = projectileinfo;
        vDir = vdir;
        bIsAlive = true;

        var sprite = AddressablesManager.Instance.GetCommon<SpriteAtlas>(AtalsName);
        if (sprite != null)
        {
            SpriteTexs = new Sprite[sprite.spriteCount];
            sprite.GetSprites(SpriteTexs);
        }

        if (spriteRenderer == null)
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if(SpriteTexs.Length > projectileinfo.iLevel)
            spriteRenderer.sprite = SpriteTexs[projectileinfo.iLevel - 1];
    }

    public void Release()
    {
        if (_releaseAct != null)
            _releaseAct.Invoke();
    }

    public void OnSpawn(Action releaseSelf)
    {
        _releaseAct = releaseSelf;
    }
}
