using Attack;
using UnityEngine;
using System;
using System.Collections;

public class Monster : BaseEntity, IDamageable, IPoolable
{
    // ?ì‹ ?´ëž˜?¤ì—???íƒœë¥?ì°¸ì¡°/?„í™˜?????ˆë„ë¡?protectedë¡??°ë‹¤
    protected enum MonsterState {Idle, Chase, Fear, Dead, End}
    
    [Header("Components")]
    [SerializeField] protected Transform _playerTransform;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] protected Material _material;
    [SerializeField] protected Rigidbody2D _rigidbody2D;

    protected static readonly int _flashAmountID = Shader.PropertyToID("_FlashAmount");
    protected static readonly int _DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    protected static readonly int _isMoveID = Animator.StringToHash("isMove");
    protected static readonly int _isFearID = Animator.StringToHash("isFear");
    protected static readonly int _directionID = Animator.StringToHash("Direction");
    
    private Action _releaseSelf;
    
    protected MonsterState _currentState = MonsterState.Idle;
    
    public void OnSpawn(Action releaseSelf) => _releaseSelf = releaseSelf;
    
    public float CriticalDamage()
    {
        throw new NotImplementedException();
    }
    
    public void Damaged(GameObject gameObject, SAttackData DamageStruct)
    {
        if (_currentState == MonsterState.Dead)
            return;

        _currentHp -= DamageStruct.iDamage * DamageStruct.iHitCount;
        StartCoroutine(HitFlash(0.1f));
        if (_currentHp <= 0)
        {
            ChangeState(MonsterState.Dead);
        }
    }
    
    public float GuardDamage()
    {
        throw new NotImplementedException();
    }

    public bool IsCritical()
    {
        throw new NotImplementedException();
    }

    public bool IsGuard()
    {
        throw new NotImplementedException();
    }
    
    public override void Initialize(ScriptableObject initRef)
    {
        if (initRef is not MonsterRef data)
        {
            Debug.LogError($"initRef is {initRef?.GetType().Name}. not a MonsterRef");
            return;
        }
        
        CurrentHp = data.HP;
        MaxHp = data.HP;
        Speed = data.Speed;
        Power = data.Power;
        
        gameObject.layer =  LayerMask.NameToLayer("Monster");

        if (_material != null)
        {
            _material.SetFloat(_flashAmountID, 0f);
            _material.SetFloat(_DissolveAmountID, 0f);
        }

        _currentState = MonsterState.Idle;
    }

    protected override void Move(Vector3 position, float speedModifier = 1f)
    {
        Vector2 currentPos = _rigidbody2D.position;
        Vector2 targetPos = Vector2.MoveTowards(currentPos, position, Speed * speedModifier * Time.fixedDeltaTime);
        _rigidbody2D.MovePosition(targetPos);

        _animator.SetBool(_isMoveID, true);
        _animator.SetFloat(_directionID, _playerTransform.position.x - transform.position.x);
    }
    
    void Start()
    {
        // ??ì¨·????ì ™??æ¿¡ì’–ì­? ??„ì‘ ??? …??ë¼± ?ê¾©íŠ‚ è«›ì†ë¸˜äºŒ?°ë’—å«???¼ë–† ï§ëš®ë±???‰ì ™
        if(InGameManager.Instance is not null)
            _playerTransform = InGameManager.Instance.GetPlayerTransform();
        
        _renderer =  GetComponent<SpriteRenderer>();
        _material = _renderer.material;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        ChangeState(MonsterState.Idle);
    }

    void Update()
    {
        switch (_currentState)
        {
            case MonsterState.Idle: TickIdle(); break;
            case MonsterState.Chase: TickChase(); break;
            case MonsterState.Fear: TickFear(); break;
            case MonsterState.Dead: break;
        }
    }

    private IEnumerator HitFlash(float duration)
    {
        float curTime = 0f;
        _material.SetFloat(_flashAmountID, 1f);
        while (curTime < duration)
        {
            curTime += Time.deltaTime;
            float amount = 1f - Mathf.Clamp(curTime / duration, 0f, 1f);
            _material.SetFloat(_flashAmountID, amount);
            yield return null;
        }
        
        _material.SetFloat(_flashAmountID, 0f); // ë£¨í”„ ?¤ì°¨ ë³´ì •

    }

    void FixedUpdate()
    {
        switch (_currentState)
        {
            case MonsterState.Chase:
                Move(_playerTransform.position, 1f);
                break;
            case MonsterState.Fear:
                Move(_playerTransform.position, -1f);
                break;
        }
    }
    
    private IEnumerator Dissolve(float duration)
    {
        InGameManager.Instance.GetPlayerStats().GetExp(1);
        
        gameObject.layer =  LayerMask.NameToLayer("Deactive");
        float curTime = 0f;
        _material.SetFloat(_DissolveAmountID, 0f);
        while (curTime < duration)
        {
            curTime += Time.deltaTime;
            float amount = Mathf.Clamp(curTime / duration, 0f, 1f);
            _material.SetFloat(_DissolveAmountID, amount);
            yield return null;
        }
        
        _material.SetFloat(_DissolveAmountID, 1f); // ?·â‘¦ë´???¼ê° è¹‚ëŒ??
        
        _releaseSelf?.Invoke();
    }

    // ?ì‹ ?´ëž˜?¤ê? ?íƒœ ?„í™˜ ?ë¦„???¼ì–´?????ˆë„ë¡?virtualë¡??°ë‹¤
    protected virtual void ChangeState(MonsterState newState)
    {
        if (_currentState == newState)
            return;

        switch (_currentState)
        {
            case MonsterState.Fear:
                _animator.SetBool(_isFearID, false);
                break;
        }
        
        _currentState = newState;

        // ?íƒœê°€ ?¤ì œë¡?ë°”ë€??œì ???ì‹ ?´ëž˜?¤ê? ì¶”ê? ì²˜ë¦¬ë¥??????ˆê²Œ ?…ì„ ?¸ì¶œ?œë‹¤
        OnStateChanged(newState);

        switch (newState)
        {
            case MonsterState.Idle:
                _animator.SetBool(_isMoveID, false);
                break;
            case MonsterState.Chase: 
                _animator.SetBool(_isMoveID, true);
                break;
            case MonsterState.Fear: 
                _animator.SetBool(_isFearID, true);
                _animator.SetBool(_isMoveID, true);
                break;
            case MonsterState.Dead: 
                _animator.SetBool(_isMoveID, false);
                StartCoroutine(Dissolve(1f));
                //TestCode
                
                break;
        }
    }

    protected virtual void OnStateChanged(MonsterState newState) { }

    private void TickIdle()
    {
        if(_playerTransform is not null)
            ChangeState(MonsterState.Chase);
    }

    private void TickChase()
    {
        if (_playerTransform is null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }
    }

    private void TickFear()
    {
        if (_playerTransform is null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }
    }
}