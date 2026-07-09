using UnityEngine;

public class ChaseState : BaseState
{
    
    private OtakuBoss _boss;

    public ChaseState(OtakuBoss boss) : base(boss)
    {
        _boss = boss;
    }

    public override void OnStateEnter()
    {
        
    }

    public override void OnStateUpdate()
    {
        
    }

    public override void OnStateFixedUpdate()
    {
        _boss.MoveToPlayer();
    }
    
    public override void OnStateExit()
    {
        
    }
    
}
