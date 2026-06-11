using Attack;
using UnityEngine;
using System;
using System.Collections;

public class Monster : BaseEntity, IDamageable, IPoolable
{
    private enum MonsterState {Idle, Chase, Fear, Dead, End}
    
    [Header("Components")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Material _material;

    private static readonly int _flashAmountID = Shader.PropertyToID("_FlashAmount");
    private static readonly int _DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    private static readonly int _isMoveID = Animator.StringToHash("isMove");
    private static readonly int _isFearID = Animator.StringToHash("isFear");
    private static readonly int _directionID = Animator.StringToHash("Direction");
    
    private Action _releaseSelf;
    
    private MonsterState _currentState = MonsterState.Idle;
    
    public void OnSpawn(Action releaseSelf) => _releaseSelf = releaseSelf;
    
    public float CriticalDamage()
    {
        throw new NotImplementedException();
    }
    
    public void Damaged(GameObject gameObject, SAttackData DamageStruct)
    {
        if (_currentState == MonsterState.Dead)
            return;
        // Debug.Log($"Damage : {DamageStruct.iDamage } \n" +
        //           $"Hit Count: { DamageStruct.iHitCount } \n" + 
        //           $"Type : { DamageStruct.AttackType.ToString() }");

        _currentHp -= DamageStruct.iDamage * DamageStruct.iHitCount;
        StartCoroutine(HitFlash(0.1f));
        if (_currentHp <= 0)
        {
            ChangeState(MonsterState.Dead);
        }
        // else
        // {
        //     Debug.Log("Damaged");
        // }
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
        transform.position = Vector3.MoveTowards(transform.position, position, Speed * speedModifier * Time.deltaTime);
        _animator.SetBool(_isMoveID, true);
        _animator.SetFloat(_directionID, _playerTransform.position.x - transform.position.x);

    }
    
    void Start()
    {
        // 나중에 수정할 로직. 이후 플레이어 위치 받아주는거 다시 만들 예정
        if(InGameManager.Instance is not null)
            _playerTransform = InGameManager.Instance.GetPlayerTransform();
        
        _renderer =  GetComponent<SpriteRenderer>();
        _material = _renderer.material;
        
        ChangeState(MonsterState.Idle);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(MonsterState.Fear);
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Damaged(this.gameObject, new SAttackData(1, 1, EAttackType.Strike));
        }

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
        
        _material.SetFloat(_flashAmountID, 0f); // 루프 오차 보정

    }
    
    private IEnumerator Dissolve(float duration)
    {
        Debug.Log("Im Dead");
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
        
        _material.SetFloat(_DissolveAmountID, 1f); // 루프 오차 보정
        
        _releaseSelf?.Invoke();
    }

    private void ChangeState(MonsterState newState)
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
                break;
        }
    }

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

        Move(_playerTransform.position, 1f);
    }

    private void TickFear()
    {
        if (_playerTransform is null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }
        
        Move(_playerTransform.position, -1f);
    }
    
    
}
