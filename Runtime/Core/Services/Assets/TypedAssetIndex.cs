using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiftyFramework.Services
{
    public abstract class TypedAssetIndex<TAssetBase> : ScriptableObject
    {
        //[SerializeField] private GameCollectableData _collectableData;
        internal abstract class Entry
        {
            public Entry (TAssetBase asset)
            {
                Asset = asset;
                OnAllocate = null;
            }
            
            public Entry ()
            {
                Asset = default;
                OnAllocate = null;
            }

            internal TAssetBase Asset;
            internal Action<TAssetBase> OnAllocate;
        }
        
        private Dictionary<Type, Entry> _dictionary;

        public TypedAssetIndex()
        {
            _dictionary = new Dictionary<Type, Entry>();
        }
    }
}