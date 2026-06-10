using System;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public event Action OnChanged; // UI용 
    [SerializeField] private float _magneticPower;    // 자석
    [SerializeField] private float _expMagnification;
    [SerializeField] private float _projectileAmount; // 투사체 증가
    [SerializeField] private float _attackRange;      // 범위 증가
    [SerializeField] private float _attackSpeed;      // 공격속도 증가
    [SerializeField] private float _projectileSpeed;  // 투사체 속도 증가
    [SerializeField] private float _duration;         // 투사체 지속시간 증가

    [SerializeField] private int _level;
    [SerializeField] private int _currentExp;
    [SerializeField] private float _maxHp;
    [SerializeField] private float _currentHp;
    [SerializeField] private float _power;
    [SerializeField] private float _speed;
    
    public PlayerStats(PlayerStatsRef refSO)
    {
        _magneticPower = refSO.MagneticPower;
        _expMagnification = refSO.EXPMagnification;
        _projectileAmount = refSO.ProjectileAmount;
        _attackRange = refSO.AttackRange;
        _attackSpeed = refSO.AttackSpeed;
        _projectileSpeed = refSO.ProjectileSpeed;
        _duration = refSO.Duration;
        _level = 0;
        _currentExp = 0;
        _maxHp = refSO.HP;
        _currentHp = refSO.HP;
        _power = refSO.Power;
        _speed = refSO.Speed;
    }

    public int Level
    {
        get => _level;
        set
        {
            if (_level == value) return;
            _level = value;
            OnChanged?.Invoke();
        }
    }
    
    public float CurrentHp
    {
        get => _currentHp;
        set
        {
            if (Mathf.Approximately(_currentHp, value)) return;
            _currentHp = value;
            OnChanged?.Invoke();
        }
    }

    public float Power
    {
        get => _power;
        set
        {
            if (Mathf.Approximately(_power, value)) return;
            _power = value;
            OnChanged?.Invoke();
        }
    }

    public float MaxHp
    {
        get => _maxHp;
        set
        {
            _maxHp = value;
            OnChanged?.Invoke();
        }
    }
    
    public float MagneticPower
    {
        get => _magneticPower;
        set
        {
            if (Mathf.Approximately(_magneticPower, value)) return;
            _magneticPower = value;
            OnChanged?.Invoke();
        }
    }
    public float ExpMagnification
    {
        get => _expMagnification;
        set
        {
            if (Mathf.Approximately(_expMagnification, value)) return;
            _expMagnification = value;
            OnChanged?.Invoke();
        }
    }

    public float ProjectileAmount
    {
        get => _projectileAmount;
        set
        {
            if (Mathf.Approximately(_projectileAmount, value)) return;
            _projectileAmount = value;
            OnChanged?.Invoke();
        }
    }

    public float AttackRange
    {
        get => _attackRange;
        set
        {
            if (Mathf.Approximately(_attackRange, value)) return;
            _attackRange = value;
            OnChanged?.Invoke();
        }
    }

    public float AttackSpeed
    {
        get => _attackSpeed;
        set
        {
            if (Mathf.Approximately(_attackSpeed, value)) return;
            _attackSpeed = value;
            OnChanged?.Invoke();
        }
    }

    public float ProjectileSpeed
    {
        get => _projectileSpeed;
        set
        {
            if (Mathf.Approximately(_projectileSpeed, value)) return;
            _projectileSpeed = value;
            OnChanged?.Invoke();
        }
    }

    public float Duration
    {
        get => _duration;
        set
        {
            if (Mathf.Approximately(_duration, value)) return;
            _duration = value;
            OnChanged?.Invoke();
        }
    }

    public int CurrentExp
    {
        get => _currentExp;
        set
        {
            if (_currentExp == value) return;
            _currentExp = value;
            OnChanged?.Invoke();
        }
    }

    public float Speed
    {
        get => _speed;
        set
        {
            if (Mathf.Approximately(_speed, value)) return;
            _speed = value;
            OnChanged?.Invoke();
        }
    }
    // expmagnification
    // projectileamount
    // attackrange
    // attackspeed
    // projectilespeed
    // duration
    // currentexp
    // speed
}
