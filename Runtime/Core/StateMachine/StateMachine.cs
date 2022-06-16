
namespace NiftyFramework.Core
{
    public class StateMachine
    {
        private IState _currentState;
        
        public void SwitchState(IState newState)
        {
            if (newState == _currentState)
            {
                return;
            }

            IState previousState = _currentState;
            _currentState = newState;

            previousState?.Exit();
            newState.Enter();
        }
    }

    public class StateMachine<TState> where TState : IState
    {
        private TState _currentState;
        
        public void SwitchState(TState newState)
        {
            if (newState.Equals(_currentState))
            {
                return;
            }

            TState previousState = _currentState;
            _currentState = newState;

            previousState?.Exit();
            newState.Enter();
        }
    }
}