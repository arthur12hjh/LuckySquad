using Item;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject WeaponObj;

    ProjectileWeapon Weapon;
    Animator playerAnimator;
    Transform playerTransform;
    Rigidbody2D playerrb;

    [SerializeField] Vector2 inputVec;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(playerObj == null)
        {
            Debug.Log($"PlayerController : Cannot find Player Object.\nInput Player Object to Inspector");
            gameObject.SetActive(false);
        }
        else
        {
            playerAnimator = playerObj.GetComponent<Animator>();
            playerTransform = playerObj.transform;
            playerrb = playerObj.GetComponent<Rigidbody2D>();

            var obj = GameObject.Instantiate(WeaponObj, playerObj.transform);

            ItemData? data = DataManager.Instance.FindItemData(4);
            if (data is ItemData Iteminfo)
            {
                Weapon = obj.GetComponent<ProjectileWeapon>();
                Weapon.Initalize(Iteminfo);
            }
        }
    }

    void Update()
    {
        if(Weapon != null)
            Weapon.Update_Directation(new Vector3(1f, 1f, 0f));
    }

    private void FixedUpdate()
    {
        playerrb.MovePosition(playerrb.position + inputVec * (Speed * Time.fixedDeltaTime));
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    public float Speed { get{return speed;} set{speed = value;} }

    private Rigidbody2D InitializeRigidbody2D(GameObject obj)
    {
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0;

        return rb;
    }

}
