using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.RuntimeData
{
    [ResetOnExitPlayMode]
    [RegisterToGlobalContainer]
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/RuntimeData/GameRuntimeData")]
    public class GameRuntimeDataSO : ScriptableObject
    {
        [field: SerializeField] public ObservableInt TotalDebrisCleared { get; private set; }

    }

}
