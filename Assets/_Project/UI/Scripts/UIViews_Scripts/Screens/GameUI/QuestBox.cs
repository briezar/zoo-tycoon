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
            QuestManager.Instance.OnQuestAccepted[this] += (info) => HandleOnQuestAccepted(info);
            QuestManager.Instance.OnQuestUpdated[this] += (quest, info) => UpdateQuestInfo(quest);

            UpdateQuestInfo(QuestManager.Instance.CurrentQuest);
        }

        private void OnDisable()
        {
            QuestManager.Instance?.OnQuestAccepted.Clear(this);
            QuestManager.Instance?.OnQuestUpdated.Clear(this);
        }

        private void UpdateQuestInfo(QuestInstance quest)
        {
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