using System;
using System.Collections.Generic;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.QuestSystem
{
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/Quest/Quest Definition", order = 0)]
    public class QuestDefinitionSO : ScriptableObject
    {
        [field: SerializeField] public string QuestId { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }

        [field: SerializeField, TextArea]
        public string Description { get; private set; }

        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: SerializeField] public QuestDefinitionSO[] PrerequisiteQuests { get; private set; }

        [field: SerializeField] public QuestObjective Objective { get; private set; }
        [field: SerializeField] public ResourceAmount[] ResourceRewards { get; private set; }

        [field: SerializeField] public string[] AcceptDialogues { get; private set; }
        [field: SerializeField] public string[] CompleteDialogues { get; private set; }

        [Button]
        private void LogObjectiveDescription()
        {
            Debug.Log(Objective.Description);
        }

    }

}
