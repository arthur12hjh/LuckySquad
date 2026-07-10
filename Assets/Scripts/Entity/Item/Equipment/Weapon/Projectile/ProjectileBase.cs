using Item;
using UnityEngine;
using System;
using UnityEngine.U2D;

public abstract class ProjectileBase : MonoBehaviour, IPoolable
{
    protected Action            _releaseAct;
    protected SpriteRenderer    spriteRenderer = null;
    protected Projectileinfo    info;

    protected Animator          animator = null;

    protected LevelData         _levelData = null;
    protected Sprite[]          SpriteTexs;

    protected Vector3           vDir = Vector3.zero;
    protected bool              bIsAlive = false;

    public void Initalize(LevelData levelData,
                        Projectileinfo projectileinfo,
                        Vector2 vdir,
                        string AtalsName,
                        string ControolerName)
    {
        _levelData = levelData;
        ShootProjectile(projectileinfo, vdir, AtalsName, ControolerName);
    }

    protected virtual void ShootProjectile( Projectileinfo projectileinfo, 
                                            Vector2 vdir, 
                                            string AtalsName,
                                            string ControolerName)
    {
        info = projectileinfo;
        vDir = vdir;
        bIsAlive = true;

        if(animator  == null)
            animator = GetComponent<Animator>();

        if (animator != null)
        {
            var Animcontroller = AddressablesManager.Instance.GetLabelDictionary<RuntimeAnimatorController>("Stage1", ControolerName);
            if (Animcontroller != null)
                animator.runtimeAnimatorController = Animcontroller ;
        }

        var sprite = AddressablesManager.Instance.GetAtlasSprite<SpriteAtlas>(AtalsName);
        if (sprite != null)
            SpriteTexs = sprite;

        if (spriteRenderer == null)
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if(SpriteTexs.Length > projectileinfo.iLevel)
            spriteRenderer.sprite = SpriteTexs[projectileinfo.iLevel - 1];
    }

    public virtual void Release()
    {
        if (_releaseAct != null)
            _releaseAct.Invoke();
    }

    public void OnSpawn(Action releaseSelf)
    {
        _releaseAct = releaseSelf;
    }
}
