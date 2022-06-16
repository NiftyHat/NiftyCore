using NiftyFramework.Core;
using NiftyFramework.Services;

namespace NiftyFramework
{
    public class NiftyTimer : IUpdateable
    {
        public struct Config
        {
            public int Loops;
            public int Max;
            public float Interval;
            public OnEventDelegate OnStart;
            public OnCompleteDelegate OnComplete;
            public OnCompleteDelegate OnLoop;
            public OnIntervalDelegate OnOnInterval;
            public OnEventDelegate OnReset;
            public OnPausedDelegate OnPauseChanged;
        }
        
        public delegate void OnIntervalDelegate(float elapsed, float max);
        public delegate void OnCompleteDelegate(int loops, float max);
        public delegate void OnPausedDelegate(bool isPaused);
        public delegate void OnEventDelegate();

        public event OnEventDelegate OnStart;
        public event OnCompleteDelegate OnComplete;
        public event OnCompleteDelegate OnLoop;
        public event OnIntervalDelegate OnInterval;
        public event OnEventDelegate OnReset;
        public event OnPausedDelegate OnPauseChanged;
        
        public event Updated OnUpdated;

        private float _elapsed = 0;

        public float Elapsed => _elapsed;
        private readonly float _progressInterval = 0;
        private float _nextProgressInterval;
        private readonly float _max = -1;
        private int _loops = 0;

        private bool _isPaused;
        public bool IsPaused => _isPaused;

        private bool _isRunning;
        public bool IsRunning => _isRunning;
        public string Name;

        private UpdateService _updateService;

        public NiftyTimer()
        {
            if (App.Services != null)
            {
                App.Services.Resolve<UpdateService>(service =>
                {
                    if (_updateService == null)
                    {
                        _updateService = service;
                        _updateService.Add(this);
                    }
                });
            }
           
        }
        
        public NiftyTimer(Config config) : this()
        {
            _max = config.Max;
            _loops = config.Loops;
            _progressInterval = _nextProgressInterval = config.Interval;
            OnComplete += config.OnComplete;
            OnLoop += config.OnLoop;
            OnInterval += config.OnOnInterval;
        }
        
        public NiftyTimer(float max, OnCompleteDelegate onComplete = null) : this()
        {
            _max = max;
            OnComplete += onComplete;
        }
        
        public NiftyTimer(OnIntervalDelegate onInterval = null, float interval = 1.0f) : this()
        {
            _progressInterval = _nextProgressInterval = interval;
            OnInterval += onInterval;
        }
        
        public NiftyTimer(float max, int loops = -1, OnCompleteDelegate onComplete = null, OnCompleteDelegate onLoop = null) : this()
        {
            _max = max;
            _loops = loops;
            OnComplete += onComplete;
            OnLoop += onLoop;
        }
        
        public NiftyTimer(float max, float interval = 1.0f, OnCompleteDelegate onComplete = null, OnIntervalDelegate onOnInterval = null) : this()
        {
            _max = max;
            _progressInterval = _nextProgressInterval = interval;
            if (onComplete != null)
            {
                Start(onComplete);
            }
            
            OnInterval += onOnInterval;
        }

        public NiftyTimer(float max, int loops = -1, float interval = 1.0f, OnCompleteDelegate onComplete = null, OnCompleteDelegate onLoop = null, OnIntervalDelegate onOnInterval = null) : this()
        {
            _max = max;
            _loops = loops;
            _progressInterval = _nextProgressInterval = interval;
            OnComplete += onComplete;
            OnLoop += onLoop;
            OnInterval += onOnInterval;
        }

        public void Start(OnCompleteDelegate onComplete = null)
        {
            _elapsed = 0;
            if (!_isRunning)
            {
                _isRunning = true;
            }
            if (_isPaused)
            {
                _isPaused = false;
                OnPauseChanged?.Invoke(false);
            }
            if (onComplete != null)
            {
                OnComplete += onComplete;
            }
            OnStart?.Invoke();
        }
        
        public void Resume()
        {
            if (_isPaused)
            {
                _isPaused = false;
                OnPauseChanged?.Invoke(false);
            }
        }

        public void Pause()
        {
            if (!_isPaused)
            {
                _isPaused = true;
                OnPauseChanged?.Invoke(true);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Reset()
        {
            _elapsed = 0;
            _nextProgressInterval = _progressInterval;
            OnReset?.Invoke();
        }

        private void Finish()
        {
            OnComplete?.Invoke(_loops, _max);
        }

        // Update is called once per frame


        public void Update(float timeDeltaTime)
        {
            if (!_isPaused && _isRunning)
            {
                _elapsed += timeDeltaTime;

                while (_progressInterval > 0 && _elapsed > _nextProgressInterval)
                {
                    _nextProgressInterval += _progressInterval;
                    OnInterval?.Invoke(_elapsed, _max);
                    OnUpdated?.Invoke();
                }
            }

            if (_max > 0 && _elapsed >= _max)
            {
                if (_loops == 0)
                {
                    _elapsed = _max;
                    Finish();
                }
                else
                {
                    if (_loops > 0)
                    {
                        _loops -= 1;
                    }
                    _elapsed = _max - _elapsed; //this will make elapsed go into negative numbers but prevents compound floating point errors.
                    OnLoop?.Invoke(_loops, _elapsed);
                }
            }
        }
    }
}