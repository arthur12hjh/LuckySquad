using Attack;
using Item;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Projectileinfo              info = new Projectileinfo();

    private Vector2                     vDir = Vector2.zero;
    private bool                        bIsAlive = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(bIsAlive)
        {
            transform.position = vDir * info.fSpeed * Time.deltaTime;
        }
    }

    public void ShootProjectile(ref Projectileinfo projectileinfo, Vector2 vdir)
    {
        info = projectileinfo;
        vDir = vdir;
        bIsAlive = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Enter : {other.name}");

        var Monster = other.gameObject.GetComponent<TestMonster>();
        if(Monster != null)
        {
            Monster.Damaged(Monster.gameObject, new SAttackData(info.fDamage, 1, EAttackType.Strike));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exit : {other.name}");
    }
}
