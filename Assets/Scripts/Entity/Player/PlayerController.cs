using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Rigidbody2D _playerrb;

    [SerializeField] Vector2 _inputVec;
    
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
        _inputVec = value.Get<Vector2>();
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
