using System;
using System.Collections.Generic;
using System.Linq;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.QuestSystem
{
    /// <summary>
    /// Runtime representation of an accepted quest.
    /// Wraps a QuestDefinition and tracks live objective progress.
    /// </summary>
    [Serializable]
    public class QuestInstance
    {
        [field: SerializeField] public QuestDefinitionSO Definition { get; private set; }

        [SerializeField] private int _current;

        public readonly SourcedAction OnQuestCompleted = new();
        public readonly SourcedAction<IntChangeInfo> OnValueChanged = new();

        public int Current
        {
            get => _current;
            set
            {
                var prev = _current;
                if (value == prev) { return; }
                _current = value;
                OnValueChanged?.Invoke(new(prev, value));

                if (Definition.Objective.IsComplete(value))
                {
                    OnQuestCompleted?.Invoke();
                }
            }
        }

        public float Progress => Definition.Objective.GetProgress(Current);
        public bool IsComplete => Definition.Objective.IsComplete(Current);

        public QuestInstance(QuestDefinitionSO definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public void ForceComplete() => Current = Definition.Objective.Target;

    }
}
