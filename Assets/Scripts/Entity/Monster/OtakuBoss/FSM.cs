using UnityEngine;

public class FSM
{
    private BaseState _currentState;
    
    public FSM(BaseState initialState)
    {
        _currentState =  initialState;
        _currentState.OnStateEnter();
    }

    public void ChangeState(BaseState nextState)
    {
        if (nextState == _currentState && !nextState.CanTransitionToSelf)
            return;
        
        if(_currentState != null)
            _currentState.OnStateExit();
        
        _currentState = nextState;
        _currentState.OnStateEnter();
    }

    public void UpdateState()
    {
        if(_currentState != null)
            _currentState.OnStateUpdate();
    }

    public void FixedUpdateState()
    {
        if(_currentState != null)
            _currentState.OnStateFixedUpdate();
    }
}
