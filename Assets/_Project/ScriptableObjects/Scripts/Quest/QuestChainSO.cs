using System;
using System.Collections.Generic;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.QuestSystem
{
    /// <summary>
    /// An ordered narrative sequence of quests.
    /// The chain auto-advances: when one quest completes, the next quest's intro dialogue fires and that quest becomes active.
    ///
    /// This is the primary tool for authored, story-driven progression
    /// (tutorial flow, campaign chapters, etc.)
    /// </summary>
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/Quest/Quest Chain", order = 1)]
    public class QuestChainSO : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }

        [field: SerializeField] public QuestChainStep[] Steps { get; private set; }

        [field: SerializeField] public float AutoPlayDelay { get; private set; } = 0f;
    }

    [Serializable]
    public class QuestChainStep
    {
        [field: SerializeField] public QuestDefinitionSO Quest { get; private set; }

        [field: SerializeField] public float StartDelay { get; private set; } = 1f;

        [field: TextArea, Tooltip("Dialogues shown before this quest is accepted (story setup).")]
        [field: SerializeField] public string[] IntroDialogues { get; private set; }

        [field: TextArea, Tooltip("Dialogue shown after the previous quest completes, before this one starts.")]
        [field: SerializeField] public string[] CompleteDialogues { get; private set; }

    }
}
