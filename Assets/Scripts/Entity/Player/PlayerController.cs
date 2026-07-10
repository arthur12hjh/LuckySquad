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

    [SerializeField] private Player _playercs;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Rigidbody2D _playerrb;

    [SerializeField] private Vector2 _inputVec;
    private Vector2 _playerDir = Vector2.left;
    public Vector2 GetPlayerDir() => _playerDir;

    private void FixedUpdate()
    {
        if (_playerrb is not null)
        {
            _playerrb.MovePosition(_playerrb.position + _inputVec * (Speed * Time.fixedDeltaTime));
            _playercs.UpdatePlayer(_inputVec);
        }
    }

    void OnMove(InputValue value)
    {
        _inputVec = value.Get<Vector2>();
        _playercs.UpdatePlayer(_inputVec);
        if(_inputVec != default)
            _playerDir = _inputVec;
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

        _playercs = playerObj.GetComponent<Player>();
        _playerTransform = _playerObj.transform;
        _playerrb = _playerObj.GetComponent<Rigidbody2D>();

        if (_playerrb == null)
        {
            Debug.LogWarning("PlayerController:: Player Prefab's Rigidbody is empty.\nPlease Make Own Rigidbody");
            _playerrb = InitializeRigidbody2D(_playerObj);
        }
    }

    public float Speed { get { return _playerStats.Speed; } set { _playerStats.Speed = value; } }

    private Rigidbody2D InitializeRigidbody2D(GameObject obj)
    {
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0;

        return rb;
    }

}