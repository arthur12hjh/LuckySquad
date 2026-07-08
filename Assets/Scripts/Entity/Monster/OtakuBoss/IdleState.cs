using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(Monster monster) : base(monster)
    {
        _canTransitionToSelf = true;
    }

    public override void OnStateEnter()
    {
        
    }

    public override void OnStateUpdate()
    {
        
    }
    
    public override void OnStateFixedUpdate()
    {
        
    }

    public override void OnStateExit()
    {
        
    }
    
}
