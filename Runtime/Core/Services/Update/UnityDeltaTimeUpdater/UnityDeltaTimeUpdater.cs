using System;
using UnityEngine;

namespace NiftyFramework.Services
{
    public class UnityDeltaTimeUpdater : MonoBehaviour
    {
        private static GameObject _instance;
        public UpdateService.OnIntervalDelegate OnUpdate;

        public static UnityDeltaTimeUpdater Instance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("DeltaTimeUpdate", new Type[]{typeof(UnityDeltaTimeUpdater)});
                DontDestroyOnLoad(_instance);
            }
            return _instance.GetComponent<UnityDeltaTimeUpdater>();
        }

        private void Update()
        {
            OnUpdate?.Invoke(Time.deltaTime);
        }
    }
}