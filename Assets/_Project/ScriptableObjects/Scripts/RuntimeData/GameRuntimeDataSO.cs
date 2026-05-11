using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GameDevKit;
using UnityEngine;
using ZooTycoon.QuestSystem;

namespace ZooTycoon.RuntimeData
{
    [ResetOnExitPlayMode]
    [RegisterToGlobalContainer]
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/RuntimeData/GameRuntimeData")]
    public class GameRuntimeDataSO : ScriptableObject
    {
        [field: SerializeField] public ObservableInt TotalDebrisCleared { get; private set; }
        [field: SerializeField] public List<HabitatDefinitionSO> AvailableHabitats { get; private set; }

        [SerializeField] private QuestObjectiveDefinitionSO _clearDebrisObjective;

        public readonly SourcedAction OnAvailableHabitatsChanged = new();

        private void OnEnable()
        {
            TotalDebrisCleared.OnValueChanged[this] += (info) => QuestManager.Instance.IncreaseObjective(_clearDebrisObjective, info.Diff);
        }

    }

}
