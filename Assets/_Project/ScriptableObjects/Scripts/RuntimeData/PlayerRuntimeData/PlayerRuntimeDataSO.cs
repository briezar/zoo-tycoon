using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.RuntimeData
{
    [ResetOnExitPlayMode]
    [RegisterToGlobalContainer]
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/PlayerRuntimeData")]
    public class PlayerRuntimeDataSO : ScriptableObject
    {
        [field: SerializeField] public PlayerResourceData ResourceData { get; private set; }

        public static PlayerRuntimeDataSO Current { get; private set; }

        private void OnEnable() => Current = this;

    }
}
