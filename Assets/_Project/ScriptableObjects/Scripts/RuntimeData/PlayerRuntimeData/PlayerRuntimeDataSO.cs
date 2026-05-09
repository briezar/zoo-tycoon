using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.RuntimeData
{
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/PlayerRuntimeData")]
    public class PlayerRuntimeDataSO : RuntimeDataSO
    {
        [field: SerializeField] public PlayerResourceData Resource { get; private set; }

        public static PlayerRuntimeDataSO Current { get; private set; }

        private void OnEnable() => Current = this;

    }
}
