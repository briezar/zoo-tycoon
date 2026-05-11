using System;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.QuestSystem
{
    /// <summary>
    /// Data describing a single objective within a quest.
    /// </summary>
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/Quest/Quest Objective Definition")]
    public class QuestObjectiveDefinitionSO : ScriptableObject
    {
        // [field: SerializeField] public int Target { get; private set; }

        // [SerializeField] private FormattableText _description;

        // [NonSerialized] private string _cachedDescription;

        // public string Description
        // {
        //     get
        //     {
        //         if (!Application.isPlaying) { _cachedDescription = null; }
        //         return _cachedDescription ??= _description.GetFormattedText(this);
        //     }
        // }

        // public float GetProgress(int current) => (float)current / Target;
        // public bool IsComplete(int current) => current >= Target;
    }

    [Serializable]
    public class QuestObjective
    {
        [field: SerializeField] public QuestObjectiveDefinitionSO Definition { get; private set; }
        [field: SerializeField] public int Target { get; private set; }

        [SerializeField] private FormattableText _description;

        [NonSerialized] private string _cachedDescription;

        public string Description
        {
            get
            {
                if (!Application.isPlaying) { _cachedDescription = null; }
                return _cachedDescription ??= _description.GetFormattedText(this);
            }
        }

        public float GetProgress(int current) => (float)current / Target;
        public bool IsComplete(int current) => current >= Target;
    }
}