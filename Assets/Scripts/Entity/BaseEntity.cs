using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    protected float _maxHp = 0f;
    protected float _currentHp = 0f;
    protected float _speed = 1f;
    protected float _power = 1f;
    
    public abstract void Initialize(ScriptableObject initRef);

    protected abstract void Move(Vector3 position, float speedModifier = 1f);

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public float MaxHp
    {
        get => _maxHp;
        set => _maxHp = value;
    }

    public float CurrentHp
    {
        get => _currentHp;
        set => _currentHp = value;
    }
    
    public float Power
    {
        get => _power;
        set => _power = value;
    }
}
