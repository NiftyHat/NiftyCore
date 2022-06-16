using System.Collections.Generic;
using NiftyFramework.Core;
using NiftyFramework.Core.Services;
using UnityEditor;
using UnityEngine;

namespace NiftyFramework.Services
{
    public class ServicesWindow : EditorWindow
    {
        private readonly ServiceImplementationProvider<NiftyService> _implementationProvider =
            new ServiceImplementationProvider<NiftyService>();
        private GenericMenu _assetMenu;
        private ServiceSet<INiftyService> _serviceSet;
        private List<ServiceImplementationProvider<NiftyService>.ItemData> _items;

        private List<INiftyService> _services;

        private void OnEnable()
        {
            _items = _implementationProvider.GetItems();
            if (App.Services != null)
            {
                HandleServicesAllocated(App.Services);
            }
            else
            {
                App.OnServicesAllocated += HandleServicesAllocated;
            }
            EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
        }

        private void HandlePlayModeStateChange(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.EnteredPlayMode && _serviceSet == null)
            {
                if (App.Services != null)
                {
                    HandleServicesAllocated(App.Services);
                }
                else
                {
                    App.OnServicesAllocated += HandleServicesAllocated;
                }
            }
        }

        private void HandleServicesAllocated(ServiceSet<INiftyService> serviceSet)
        {
            if (_serviceSet == null || serviceSet != _serviceSet)
            {
                _serviceSet = serviceSet;
                _services = _serviceSet.All();
                _serviceSet.OnChange += HandleServicesChanged;
            }
        }

        private void HandleServicesChanged()
        {
            _services = _serviceSet.All();
        }

        void OnGUI () 
        {
            if (_services != null)
            {
                foreach (var item in _services)
                {
                    GUI.color = Color.cyan;
                    GUILayout.Label(item.GetType().ToString());
                }
            }
            foreach (var item in _items)
            {
                GUI.color = Color.white;
                GUILayout.Label(item.DisplayName);
            }
        }
        
        [MenuItem ("NiftyFramework/Services")]
        public static void  ShowWindow () 
        {
            EditorWindow.GetWindow(typeof(ServicesWindow));
        }
    }
}