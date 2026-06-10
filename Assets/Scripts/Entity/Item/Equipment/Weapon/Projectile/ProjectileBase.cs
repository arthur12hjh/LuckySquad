using Item;
using UnityEngine;
using System;

public abstract class ProjectileBase : MonoBehaviour, IPoolable
{
    protected Action            _releaseAct;
    protected SpriteRenderer    spriteRenderer = null;
    protected Projectileinfo    info = new Projectileinfo();
    protected Vector3           vDir = Vector2.zero;
    protected bool              bIsAlive = false;

    public virtual void ShootProjectile(Projectileinfo projectileinfo, Vector2 vdir, Sprite Tex)
    {
        info = projectileinfo;
        vDir = vdir;
        bIsAlive = true;

        if (spriteRenderer == null)
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = Tex;
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
