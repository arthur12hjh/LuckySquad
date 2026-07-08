public abstract class BaseState
{
    protected Monster _monster;

    protected bool _canTransitionToSelf = false;
    
    public bool CanTransitionToSelf => _canTransitionToSelf;
    
    protected BaseState(Monster monster)
    {
        _monster = monster;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateFixedUpdate();
    public abstract void OnStateExit();
}
