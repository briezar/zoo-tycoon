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

        [SerializeField] private QuestObjectiveDefinitionSO _clearDebrisObjective;

        private void OnEnable()
        {
            TotalDebrisCleared.OnValueChanged[this] += (info) =>
            {
                foreach (var quest in QuestManager.Instance.AcceptedQuests)
                {
                    if (quest.Definition.Objective.Definition == _clearDebrisObjective)
                    {
                        quest.Current += info.Diff;
                    }
                }
            };
        }

    }

}
