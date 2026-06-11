using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Item;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class WeaponPrefabPair
    {
        public EWeaponType weaponType;
        public GameObject prefab;
    }

    [SerializeField] private GameObject _playerObj;
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Rigidbody2D _playerrb;

    [SerializeField] private List<WeaponPrefabPair> _weaponPrefab;
    [SerializeField] private Dictionary<EWeaponType, GameObject> _weaponDic;
    [SerializeField] private Dictionary<EWeaponType, GameObject> _equipmentItems;


    public Vector2 Look => _inputVec;
    [SerializeField] private Vector2 _inputVec;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
      
    }

    private void FixedUpdate()
    {
        if(_playerrb != null)
            _playerrb.MovePosition(_playerrb.position + _inputVec * (Speed * Time.fixedDeltaTime));
    }

    void OnMove(InputValue value)
    {
        Vector2 chanbin = value.Get<Vector2>();
        if(Mathf.Approximately(chanbin.magnitude, 0f))
        {
            _inputVec = value.Get<Vector2>();
            if (_weaponDic.TryGetValue(EWeaponType.Projectile, out var obj))
            {
                obj.GetComponent<ProjectileWeapon>().Update_Directation(_inputVec);
            }
        }
            
        
    }

    public void Initialize(PlayerStats playerStats, GameObject playerObj)
    {
        _playerStats = playerStats;
        _playerObj = playerObj;
        if (playerObj == null)
        {
            enabled = false;
            return;
        }
        
        _playerAnimator = _playerObj.GetComponent<Animator>();
        _playerTransform = _playerObj.transform;
        _playerrb = _playerObj.GetComponent<Rigidbody2D>();

        if (_playerrb == null)
        {
            Debug.LogWarning("PlayerController:: Player Prefab's Rigidbody is empty.\nPlease Make Own Rigidbody");
            _playerrb = InitializeRigidbody2D(_playerObj);
        }

        _weaponDic = _weaponPrefab.ToDictionary(x => x.weaponType, x => x.prefab);

        if (_weaponDic.TryGetValue(EWeaponType.Bounce, out var obj))
        {
            _equipmentItems.Add(EWeaponType.Bounce, ItemFactory.AbstractCreateItem(obj, _playerObj.transform, 4));
        }
           
    }
    
    public float Speed { get{return _playerStats.Speed;} set{_playerStats.Speed = value;} }

    private Rigidbody2D InitializeRigidbody2D(GameObject obj)
    {
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0;

        return rb;
    }

}
