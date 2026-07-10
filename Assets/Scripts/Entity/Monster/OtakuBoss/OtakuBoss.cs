using UnityEngine;

public class OtakuBoss : Monster
{
    [SerializeField] private float _chaseRange = 1.7f;
    [SerializeField] private float _skillRange = 2f;

    [Header("Jump Skill")]
    [SerializeField] private float _jumpCooldown = 10f;

    private static readonly int _isJumpID = Animator.StringToHash("isJump");
    private static readonly int _readyToJumpID = Animator.StringToHash("readytojump");

    private int _defaultSortingLayerID;
    private int _monsterLayer;
    private int _deactiveLayer;

    private float _nextJumpReadyTime = 0f;
    private bool _isSkillFinished = false;

    private enum OtakuBossSkill
    {
        Jump,
    }

    private FSM _fsm;

    public bool IsJumpReady => Time.time >= _nextJumpReadyTime;

    public Vector2 Position => _rigidbody2D.position;

    public Vector2 PlayerPosition
    {
        get
        {
            if (_playerTransform is null)
                return _rigidbody2D.position;

            return _playerTransform.position;
        }
    }

    public override void Initialize(ScriptableObject initRef)
    {
        base.Initialize(initRef);

        _nextJumpReadyTime = 0f;
        _isSkillFinished = false;

        if (_fsm != null)
            _fsm.ChangeState(new IdleState(this));
    }

    protected override void Start()
    {
        if (InGameManager.Instance is not null)
            _playerTransform = InGameManager.Instance.GetPlayerTransform();

        _renderer = GetComponent<SpriteRenderer>();
        _material = _renderer.material;
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _defaultSortingLayerID = _renderer.sortingLayerID;
        _monsterLayer = LayerMask.NameToLayer("Monster");
        _deactiveLayer = LayerMask.NameToLayer("Deactive");

        _currentState = MonsterState.Idle;
        _fsm = new FSM(new IdleState(this));
    }

    protected override void Update()
    {
        switch (_currentState)
        {
            case MonsterState.Idle:
                TickIdle();
                break;
            case MonsterState.Chase:
                TickChase();
                break;
            case MonsterState.Skill:
                TickSkill();
                break;
            case MonsterState.Dead:
                break;
        }

        _fsm.UpdateState();
    }

    protected override void FixedUpdate()
    {
        _fsm.FixedUpdateState();
    }

    protected override void OnStateChanged(MonsterState nextState)
    {
        switch (_currentState)
        {
            case MonsterState.Idle:
                _fsm.ChangeState(new IdleState(this));
                break;
            case MonsterState.Chase:
                _fsm.ChangeState(new ChaseState(this));
                break;
            case MonsterState.Skill:
                _isSkillFinished = false;
                _fsm.ChangeState(new JumpState(this));
                break;
            case MonsterState.Dead:
                _fsm.ChangeState(new DeadState(this));
                break;
        }
    }

    protected override void TickIdle()
    {
        if (_playerTransform is not null)
            ChangeState(MonsterState.Chase);
    }

    protected override void TickChase()
    {
        if (_playerTransform is null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }
        
        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        if (distance < _chaseRange && IsJumpReady)
            ChangeState(MonsterState.Skill);
    }

    protected override void TickSkill()
    {
        if (!_isSkillFinished)
            return;

        if (_playerTransform is null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        if (distance > _skillRange)
        {
            ChangeState(MonsterState.Chase);
            return;
        }

        if (IsJumpReady)
        {
            _isSkillFinished = false;
            _fsm.ChangeState(new JumpState(this));
        }
    }

    public void MoveToPlayer()
    {
        if (_playerTransform is null)
            return;

        Move(_playerTransform.position, 1f);
    }

    //
    public void JumpTo(Vector2 position)
    {
        _rigidbody2D.MovePosition(position);
    }

    public void Teleport(Vector2 position)
    {
        _rigidbody2D.position = position;
    }

    public void PlayReadyToJump()
    {
        _animator.SetBool(_isMoveID, false);
        _animator.SetBool(_isJumpID, true);
        _animator.SetTrigger(_readyToJumpID);
    }

    public void SetJumpFlyState(bool isShadow)
    {
        _animator.SetBool(_isJumpID, isShadow);

        if (isShadow)
            _renderer.sortingLayerName = "Priority";
        else
            _renderer.sortingLayerID = _defaultSortingLayerID;
    }

    public void SetLayerByJumpFly(bool isEnabled)
    {
        if (isEnabled)
            gameObject.layer = _monsterLayer;
        else
            gameObject.layer = _deactiveLayer;
    }

    public void StartJumpCooldown()
    {
        _nextJumpReadyTime = Time.time + _jumpCooldown;
    }

    // JumpState가 착지 경직까지 마쳤을 때 호출한다
    public void OnSkillFinished()
    {
        _isSkillFinished = true;
    }
}