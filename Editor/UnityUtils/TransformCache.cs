using System;
using UnityEngine;

namespace NiftyFramework.UnityUtils
{
    [Serializable]
    public struct TransformCache
    {
        private readonly Vector3 _position;
        private readonly Quaternion _rotation;
        private readonly Vector3 _localScale;
        private readonly Transform _parent;

        public TransformCache(Transform transform)
        {
            _position = transform.position;
            _rotation = transform.rotation;
            _localScale = transform.localScale;
            _parent = transform.parent;
        }

        public void Apply(Transform target)
        {
            target.rotation = _rotation;
            target.position = _position;
            target.localScale = _localScale;
            if (target.parent != _parent)
            {
                Debug.LogWarning($"{nameof(TransformCache)}{nameof(Apply)}() {target.gameObject.name} parent has changed.");
            }
        }
    }
}