using Item;
using UnityEngine;

public class game : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
         DataManager Ins = DataManager.Instance;
        ItemFactory.AbstractCreateItem<ProjectileWeapon>(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject gameobject = GameObject.Instantiate(prefab);

            Projectileinfo info = new Projectileinfo();
            info.fSpeed = 0.015f;

            gameobject.GetComponent<Projectile>().ShootProjectile(ref info, new Vector2(1, 1));
        }
    }
}
