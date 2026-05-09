using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.RuntimeData
{
    [ResetOnExitPlayMode]
    public abstract class RuntimeDataSO : ScriptableObject
    {
        [field: SerializeField] public bool RegisterToGlobalContainer { get; private set; } = true;
    }
}
