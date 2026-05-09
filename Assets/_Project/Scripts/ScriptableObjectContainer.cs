using System;
using System.Collections.Generic;
using EditorAttributes;
using GameDevKit;
using GameDevKit.Editor;
using UnityEngine;
using ZooTycoon.RuntimeData;

namespace ZooTycoon
{
    public class RegisterToGlobalContainerAttribute : Attribute { }

    public class ScriptableObjectContainer : SingletonBehaviour<ScriptableObjectContainer>
    {
        [SerializeField] private List<ScriptableObject> _scriptableObjects = new();

        public static T Find<T>() where T : ScriptableObject
        {
            var obj = _instance == null ? null : _instance._scriptableObjects.Find(r => r is T) as T;
            if (obj == null) { Debug.LogWarning($"ScriptableObject {typeof(T).Name} is not registered!"); }
            return obj;
        }

        public static bool AssignIfNull<T>(ref T data) where T : ScriptableObject
        {
            if (_instance == null) { return false; }
            if (data == null)
            {
                data = Find<T>();
                return data != null;
            }
            return false;
        }

#if UNITY_EDITOR
        [Button]
        private void FindAndRegister()
        {
            _scriptableObjects.Clear();
            var scriptableObjects = EditorUtils.FindAssets<ScriptableObject>();
            foreach (var scriptableObject in scriptableObjects)
            {
                if (scriptableObject.HasAttribute<RegisterToGlobalContainerAttribute>())
                {
                    _scriptableObjects.Add(scriptableObject);
                }
            }

            Debug.Log($"Registered {_scriptableObjects.Count} ScriptableObjects to global container", this);
        }
#endif
    }
}