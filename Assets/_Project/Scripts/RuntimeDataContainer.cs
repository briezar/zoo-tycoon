using System.Collections.Generic;
using EditorAttributes;
using GameDevKit;
using GameDevKit.Editor;
using UnityEngine;
using ZooTycoon.RuntimeData;

namespace ZooTycoon
{
    public class RuntimeDataContainer : SingletonBehaviour<RuntimeDataContainer>
    {
        [SerializeField] private List<RuntimeDataSO> _runtimeDatas = new();

        public static T FindData<T>() where T : RuntimeDataSO => _instance == null ? null : _instance._runtimeDatas.Find(r => r is T) as T;

#if UNITY_EDITOR
        [Button]
        private void FindAndRegister()
        {
            _runtimeDatas.Clear();
            var runtimeDatas = EditorUtils.FindAssets<RuntimeDataSO>();
            foreach (var runtimeData in runtimeDatas)
            {
                if (runtimeData.RegisterToGlobalContainer)
                {
                    _runtimeDatas.Add(runtimeData);
                }
            }

            Debug.Log($"Registered {_runtimeDatas.Count} RuntimeDatas");
        }
#endif
    }
}