using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameDevKit;
using PrimeTween;
using TMPro;
using UnityEngine;
using ZooTycoon.QuestSystem;

namespace ZooTycoon.UI
{
    public class QuestBox : AdvancedBehaviour
    {
        [SerializeField] private RectTransform _questEntry;
        [SerializeField] private TMP_Text _questInfoText;

        protected override void OnStartOrEnable()
        {
            QuestRegistrySO.Instance.OnQuestAccepted[this] += (info) => HandleOnQuestAccepted(info);
            QuestRegistrySO.Instance.OnQuestUpdated[this] = (quest, info) => UpdateQuestInfo(quest, info);
            QuestRegistrySO.Instance.OnQuestUpdated.InvokeLatest(this);
        }

        private void OnDisable()
        {
            QuestRegistrySO.Instance?.OnQuestAccepted.UnsubscribeSource(this);
            QuestRegistrySO.Instance?.OnQuestUpdated.UnsubscribeSource(this);
        }

        private void UpdateQuestInfo(QuestInstance quest, IntChangeInfo? info = null)
        {
            if (QuestRegistrySO.Instance.CurrentQuest != quest) { return; }

            var text = quest == null
                ? "Nothing to do now"
                : $"{quest.Definition.Objective.Description}\n({quest.Current}/{quest.Definition.Objective.Target})";

            _questInfoText.text = text;
        }

        private async UniTask HandleOnQuestAccepted(QuestInstance quest)
        {
            await Tween.Scale(_questEntry, Vector3.zero, 0.5f, Ease.InBack);
            UpdateQuestInfo(quest);
            await Tween.Scale(_questEntry, Vector3.one, 0.5f, Ease.OutBack);
        }

    }
}