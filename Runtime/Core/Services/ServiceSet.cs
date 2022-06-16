using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NiftyFramework.Core.Services
{
    public class ServiceSet<TServiceBase>
    {
        public delegate void HandleServiceGet<in TService>(TService service);
        public delegate void HandleChange();

        public bool _isLogging = true;
        public HashSet<Type> _serviceAsyncResolving = new HashSet<Type>();
        public event HandleChange OnChange;
        internal class Entry
        {
            public Entry (TServiceBase service)
            {
                Service = service;
                OnAllocate = null;
            }
            
            public Entry ()
            {
                Service = default;
                OnAllocate = null;
            }

            internal TServiceBase Service;
            internal Action<TServiceBase> OnAllocate;
        }
        
        private Dictionary<Type, Entry> _dictionary;

        public ServiceSet()
        {
            _dictionary = new Dictionary<Type, Entry>();
        }

        public void Resolve<TService>(HandleServiceGet<TService> onGet) where TService : class, TServiceBase
        {
            Type serviceKey = typeof(TService);
            Entry entry;
            if (!_dictionary.ContainsKey(serviceKey))
            {
                entry = new Entry();
                if (_isLogging)
                {
                    Debug.Log($"ServiceSet ResolveAsync for {serviceKey} started");
                }

                _serviceAsyncResolving.Add(serviceKey);
                entry.OnAllocate += delegate(TServiceBase service)
                {
                    onGet(service as TService);
                    entry.OnAllocate = null;
                    if (_isLogging)
                    {
                        Debug.Log($"GetAsync for {serviceKey} finished. {_serviceAsyncResolving.Count} items remain");
                    }
                    _serviceAsyncResolving.Remove(serviceKey);
                };
                _dictionary[serviceKey] = entry;
            }
            else
            {
                entry = _dictionary[serviceKey];
                if (entry.Service != null)
                {
                    onGet(entry.Service as TService);
                }
                else
                {
                    if (_isLogging)
                    {
                        Debug.Log($"ServiceSet ResolveAsync for {serviceKey} started");
                    }
                    _serviceAsyncResolving.Add(serviceKey);
                    entry.OnAllocate += delegate(TServiceBase service)
                    {
                        onGet(service as TService);
                        entry.OnAllocate = null;
                        _serviceAsyncResolving.Remove(serviceKey);
                        if (_isLogging)
                        {
                            Debug.Log($"GetAsync for {serviceKey} finished. {_serviceAsyncResolving.Count} items remain");
                        }
                    };
                }
            }
        }

        public void Register<TService>() where TService : class, TServiceBase, new()
        {
            Register(new TService());
        }
        
        public void Register<TService>(TService instance, Action<TServiceBase> onAllocate = null) where TService : TServiceBase
        {
            Type serviceKey = typeof(TService);
            if (!_dictionary.ContainsKey(serviceKey))
            {
                _dictionary.Add(serviceKey, new Entry(instance));
                OnChange?.Invoke();
            }
            else
            {
                Entry entry = _dictionary[serviceKey];
                if (entry.Service == null)
                {
                    Debug.Log($"Allocated {serviceKey}");
                    entry.Service = instance;
                    entry.OnAllocate?.Invoke(entry.Service);
                    onAllocate?.Invoke(entry.Service);
                    OnChange?.Invoke();
                }
                else
                {
                    throw new Exception($"ServiceSet Add exception service {serviceKey} was already allocated with {entry.Service}");
                }
            }
        }

        public List<TServiceBase> All()
        {
            return _dictionary.Values.Select(item => item.Service).ToList();
        }

        public void Clear()
        {
            _dictionary = new Dictionary<Type, Entry>();
        }
    }
}