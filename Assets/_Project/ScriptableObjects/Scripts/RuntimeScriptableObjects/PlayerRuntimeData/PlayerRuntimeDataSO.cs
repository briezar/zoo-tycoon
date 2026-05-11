using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.RuntimeData
{
    [ResetOnExitPlayMode]
    [RegisterToGlobalContainer]
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/RuntimeData/PlayerRuntimeData")]
    public class PlayerRuntimeDataSO : ScriptableObject
    {
        [field: SerializeField] public PlayerResourceData ResourceData { get; private set; }

    }
}
