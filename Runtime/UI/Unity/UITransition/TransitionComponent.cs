using System;
using System.Collections;
using UnityEngine;

public class TransitionComponent : MonoBehaviour
{
    public class AnimatorStateTransition : ITransition
    {
        private Animator _animator;
        private AnimatorStateReference _stateReference;
        private Coroutine _coroutineStateUpdate;
        private TransitionComponent _transitionComponent;
        private bool _isPlaying;

        public AnimatorStateTransition(Animator animator, AnimatorStateReference stateReference,
            TransitionComponent transitionComponent)
        {
            _animator = animator;
            _stateReference = stateReference;
            _transitionComponent = transitionComponent;
        }

        public void Play()
        {
            if (_isPlaying)
            {
                return;
            }

            if (_stateReference.TryGetStateID(out var stateId))
            {
                if (_stateReference.TryGetLayerIndex(out int layerIndex, _animator))
                {
                    _animator.Play(stateId, layerIndex);
                    _coroutineStateUpdate = _transitionComponent.StartCoroutine(
                        WaitForAnimation(_animator, _stateReference.StateName, layerIndex));
                }
                else
                {
                    _animator.Play(stateId);
                    _coroutineStateUpdate = _transitionComponent.StartCoroutine(
                        WaitForAnimation(_animator, _stateReference.StateName));
                }

                _animator.Play(stateId);
            }
        }

        protected IEnumerator WaitForAnimation(Animator animator, string stateName, int layerIndex = 0)
        {
            if (string.IsNullOrWhiteSpace(stateName) ||
                animator == null)
            {
                yield return null;
            }
            else
            {
                //yield for the animation to start.
                AnimatorStateInfo currentState;
                do
                {
                    currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);
                    yield return null;
                } while (!currentState.IsName(stateName));

                _isPlaying = true;
                OnStarted?.Invoke(this);
                do
                {
                    currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);
                    OnProgress?.Invoke(this, currentState.normalizedTime);
                    yield return null;
                } while (currentState.IsName(stateName) && currentState.normalizedTime < 1);

                OnFinished?.Invoke(this);
                _isPlaying = false;
                //animation has finished
            }
        }

        public void Stop()
        {
            if (_transitionComponent != null && _coroutineStateUpdate != null)
            {
                _transitionComponent.StopCoroutine(_coroutineStateUpdate);
            }
        }

        private void StopStateCheckCoroutine()
        {
            if (_transitionComponent != null && _coroutineStateUpdate != null)
            {
                _transitionComponent.StopCoroutine(_coroutineStateUpdate);
            }
        }

        public void Skip()
        {
            if (!_isPlaying)
            {
                return;
            }
            if (_stateReference.TryGetLayerIndex(out int layerIndex, _animator))
            {
                StopStateCheckCoroutine();
                _animator.Play(_stateReference.StateName, layerIndex, 1.0f);
                _isPlaying = false;
            }
            else
            {
                StopStateCheckCoroutine();
                _animator.Play(_stateReference.StateName, 0, 1.0f);
                _isPlaying = false;
            }
        }

        public event Action<ITransition> OnStarted;
        public event Action<ITransition> OnFinished;
        public event Action<ITransition, float> OnProgress;
    }

    [SerializeField] private Animator _animator;
    [SerializeField] private AnimatorStateReference _animationStateIn;
    [SerializeField] private AnimatorStateReference _animationStateOut;

    protected AnimatorStateTransition _animateIn;
    protected AnimatorStateTransition _animateOut;

    void Start()
    {
        _animateIn = new AnimatorStateTransition(_animator, _animationStateIn, this);
        _animateOut = new AnimatorStateTransition(_animator, _animationStateOut, this);
        _animateIn.OnStarted += (ITransition transition) => Debug.Log($"{transition} started");
        _animateIn.OnProgress += (ITransition transition, float progress) =>
            Debug.Log($"{transition} Progress {progress:P0}");
        _animateIn.OnFinished += (ITransition transition) =>
        {
            Debug.Log($"{transition} Finished");
            //_animateOut.Play();
        };
    }

    // Update is called once per frame
    void Update()
    {
    }

    [ContextMenu("Skip")]
    void Skip()
    {
        _animateIn.Skip();
    }

    [ContextMenu("Animate In")]
    void AnimateIn()
    {
        _animateIn.Play();
    }
}