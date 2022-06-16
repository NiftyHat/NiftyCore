using System;
using UnityEngine;

namespace NiftyFramework.Core.PrefabReferences
{
    [Serializable]
    public class PrefabReference<TRequiredMono> : PrefabReference where TRequiredMono : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        
        public override bool ValidatePrefab(GameObject prefab)
        {
            TRequiredMono requiredComponent = prefab.GetComponent<TRequiredMono>();
            return requiredComponent != null;
        }

        public override void SetReference(GameObject value)
        {
            _prefab = value;
        }
    }

    public abstract class PrefabReference
    {
        public abstract bool ValidatePrefab(GameObject prefab);

        public abstract void SetReference(GameObject value);

    }
}