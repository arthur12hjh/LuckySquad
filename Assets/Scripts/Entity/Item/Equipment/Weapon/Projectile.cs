using Attack;
using Item;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    private Action                      _releaseAct;
    private SpriteRenderer              spriteRenderer = null;
    private Projectileinfo              info = new Projectileinfo();

    private Vector3                     vDir = Vector2.zero;
    private bool                        bIsAlive = false;


    // Update is called once per frame
    void Update()
    {
        if(bIsAlive)
        {
            transform.position += vDir * info.fSpeed * Time.deltaTime;
        }
    }

    public void Initalize()
    {

    }

    public void ShootProjectile(Projectileinfo projectileinfo, Vector2 vdir, Sprite Tex)
    {
        info = projectileinfo;
        vDir = vdir;
        bIsAlive = true;

        if (spriteRenderer == null)
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = Tex;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Enter : {other.name}");

        var Monster = other.gameObject.GetComponent<TestMonster>();
        if(Monster != null)
        {
            Monster.Damaged(Monster.gameObject, new SAttackData(info.fDamage, 1, EAttackType.Strike));
            Release();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exit : {other.name}");
    }

    public void Release()
    {
        if(_releaseAct != null)
            _releaseAct.Invoke();
    }

    public void OnSpawn(Action releaseSelf)
    {
        _releaseAct = releaseSelf;
    }
}
