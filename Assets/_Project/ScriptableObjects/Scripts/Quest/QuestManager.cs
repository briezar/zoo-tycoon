using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GameDevKit;
using UnityEngine;
using ZooTycoon.RuntimeData;

namespace ZooTycoon.QuestSystem
{
    public class QuestManager : SingletonBehaviour<QuestManager>
    {
        [Header("Optional")]
        [SerializeField] private PlayerRuntimeDataSO _playerData;

        public static QuestManager Instance => _instance;

        public readonly SourcedAction<QuestInstance> OnQuestAccepted = new();
        public readonly SourcedAction<QuestInstance> OnQuestCompleted = new();
        public readonly SourcedAction<QuestInstance, IntChangeInfo> OnQuestUpdated = new();

        public QuestInstance CurrentQuest { get; private set; }

        private readonly List<QuestDefinitionSO> _completedQuests = new();

        [SerializeField]
        private List<QuestInstance> _acceptedQuests = new();

        public IReadOnlyList<QuestInstance> AcceptedQuests => _acceptedQuests;

        private void Start()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerData);
        }

        /// <summary>Try to accept a quest. Returns null if prerequisites are not met.</summary>
        public bool TryAcceptQuest(QuestDefinitionSO definition, out QuestInstance quest)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));

            quest = null;
            foreach (var prerequisite in definition.PrerequisiteQuests)
            {
                if (!_completedQuests.Contains(prerequisite))
                {
                    Debug.LogWarning($"Required quest '{prerequisite}' not completed.");
                    return false;
                }
            }

            quest = GetQuest(definition);

            CurrentQuest = quest;
            OnQuestAccepted?.Invoke(quest);
            Debug.Log($"Quest accepted: {definition.DisplayName}");
            return true;
        }

        public QuestInstance GetQuest(QuestDefinitionSO definition)
        {
            var quest = _acceptedQuests.Find(q => q.Definition == definition);
            if (quest == null)
            {
                quest = new QuestInstance(definition);
                _acceptedQuests.Add(quest);

                quest.OnValueChanged[this] += (info) => OnQuestUpdated?.Invoke(quest, info);
                quest.OnQuestCompleted[this] += () => HandleQuestCompleted(quest);
            }
            return quest;
        }

        public void IncreaseObjective(QuestObjectiveDefinitionSO objectiveDef, int amount = 1)
        {
            foreach (var quest in _acceptedQuests)
            {
                if (quest.Definition.Objective.Definition == objectiveDef)
                {
                    quest.Current += amount;
                }
            }
        }

        public bool IsCompleted(QuestDefinitionSO questDef) => GetQuest(questDef).IsComplete;

        private void HandleQuestCompleted(QuestInstance quest)
        {
            _completedQuests.Add(quest.Definition);
            GrantRewards(quest.Definition);

            OnQuestCompleted?.Invoke(quest);
            Debug.Log($"Quest completed: {quest.Definition.DisplayName}");
        }

        protected virtual void GrantRewards(QuestDefinitionSO questDef)
        {
            _playerData.ResourceData.AddResources(questDef.ResourceRewards);
        }
    }
}
