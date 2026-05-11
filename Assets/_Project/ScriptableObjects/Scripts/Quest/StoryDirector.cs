using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameDevKit;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using ZooTycoon.UI;

namespace ZooTycoon.QuestSystem
{
    public class StoryDirector : MonoBehaviour
    {
        [field: SerializeField] public QuestChainSO QuestChain { get; private set; }

        [field: Header("In-game tracking")]
        [field: SerializeField] public int CurrentIndex { get; private set; } = -1;

        public static StoryDirector current { get; private set; }

        public static readonly SourcedAction<QuestDefinitionSO> OnStepIntroStarted = new();
        public static readonly SourcedAction<ValueChangeInfo<QuestInstance>> OnStepStarted = new();
        public static readonly SourcedAction<QuestInstance> OnStepCompleted = new();
        public static readonly SourcedAction<QuestChainSO> OnQuestChainCompleted = new();

        private DialogueOverlay _dialoguePopup;

        private void OnEnable() => current = this;

        private IEnumerator Start()
        {
            yield return YieldCollection.WaitForSeconds(QuestChain.AutoPlayDelay);
            StartQuestChain();
        }

        public void StartQuestChain()
        {
            StartCoroutine(PlayStepRoutine(0));
        }

        private IEnumerator PlayStepRoutine(int index)
        {
            CurrentIndex = index;

            var step = QuestChain.Steps[index];

            OnStepIntroStarted?.Invoke(step.Quest);

            yield return YieldCollection.WaitForSeconds(step.StartDelay);

            yield return PlayDialoguesRoutine(step.IntroDialogues);

            QuestManager.Instance.TryAcceptQuest(step.Quest, out var quest);

            OnStepStarted?.Invoke(new(null, quest));

            object source = new();
            quest.OnQuestCompleted[source] += () =>
            {
                quest.OnQuestCompleted.Clear(source);

                StartCoroutine(QuestCompleteRoutine());
                IEnumerator QuestCompleteRoutine()
                {
                    yield return PlayDialoguesRoutine(step.CompleteDialogues);
                    OnStepCompleted?.Invoke(quest);

                    var completedChain = CurrentIndex == QuestChain.Steps.Length - 1;
                    if (!completedChain)
                    {
                        StartCoroutine(PlayStepRoutine(CurrentIndex + 1));
                        yield break;
                    }

                    OnQuestChainCompleted?.Invoke(QuestChain);
                }
            };
        }

        private IEnumerator WaitForAnyKeyPressed()
        {
            var pressed = false;
            var anyKeyEvent = InputSystem.onAnyButtonPress.Call((t) => pressed = true);
            yield return YieldCollection.WaitUntil(() => pressed);
        }

        private IEnumerator PlayDialoguesRoutine(IEnumerable<string> dialogueTexts)
        {
            foreach (var text in dialogueTexts)
            {
                yield return PlayDialogueRoutine(text);
                yield return WaitForAnyKeyPressed();
            }

            yield return UIManager.HideUI<DialogueOverlay>().ToCoroutine();
            _dialoguePopup = null;
        }

        private IEnumerator PlayDialogueRoutine(string dialogueText)
        {
            if (_dialoguePopup == null)
            {
                _dialoguePopup = UIManager.ShowUI<DialogueOverlay>();
                _dialoguePopup.DialogueText = "";
                yield return YieldCollection.WaitForSeconds(0.5f);
            }
            yield return _dialoguePopup.TypeText(dialogueText).ToCoroutine();
        }

    }
}