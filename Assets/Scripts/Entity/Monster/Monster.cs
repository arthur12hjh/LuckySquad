using UnityEngine;

public class Monster : BaseEntity
{
    
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
    }

    protected override void Move()
    {
        
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
