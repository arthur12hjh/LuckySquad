using Item;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Projectileinfo              info = new Projectileinfo();
    private Rigidbody2D                 rb2D = null;

    private Vector2                     vDir = Vector2.zero;
    private bool                        bIsAlive = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(bIsAlive)
        {
            rb2D.AddForce(vDir * info.fSpeed, ForceMode2D.Impulse);
        }
    }

    public void ShootProjectile(ref Projectileinfo settinginfo, Vector2 vdir)
    {
        info = settinginfo;
        vDir = vdir;
        bIsAlive = true;
    }
}
