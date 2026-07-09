using UnityEngine;

public class OtakuBoss : Monster
{
    [SerializeField] private float _chaseRange = 1.7f;
    [SerializeField] private float _skillRange = 2f;
    
    private enum OtakuBossSkill
    {
        Jump,
    }

    private FSM _fsm;

    public override void Initialize(ScriptableObject initRef)
    {
        base.Initialize(initRef);

        if (_fsm != null)
            _fsm.ChangeState(new IdleState(this));
    }
    
    protected override void Start()
    {
        if(InGameManager.Instance is not null)
            _playerTransform = InGameManager.Instance.GetPlayerTransform();
        
        _renderer =  GetComponent<SpriteRenderer>();
        _material = _renderer.material;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        _currentState = MonsterState.Idle;
        _fsm = new FSM(new IdleState(this));
    }

    protected override void Update()
    {
        // State 전환
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

        // 플레이어와의 거리가 _chaseRange 안으로 들어오면 스킬 상태로 전환한다
        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        if (distance < _chaseRange)
            ChangeState(MonsterState.Skill);
    }
    
    protected override void TickSkill()
    {
        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        if (distance > _skillRange)
            ChangeState(MonsterState.Chase);
    }
    
    public void MoveToPlayer()
    {
        if (_playerTransform is null)
            return;

        Move(_playerTransform.position, 1f);
    }
    
}
