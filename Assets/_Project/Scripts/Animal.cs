using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameDevKit;
using PrimeTween;
using UnityEngine;
using UnityEngine.AI;
using ZooTycoon.Input;
using ZooTycoon.QuestSystem;
using ZooTycoon.RuntimeData;

namespace ZooTycoon
{
    public class Animal : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public AnimalInteractionConfig InteractionConfig { get; private set; }
        [field: SerializeField] public AnimalDefinitionSO Definition { get; private set; }
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public AnimalInteractionUI InteractionUI { get; private set; }
        [field: SerializeField] public bool IsCaptured { get; private set; }

        [SerializeField] private Animator _animator;
        [SerializeField] private SerializableAnimationHash _moveSpeedParam;

        [SerializeField] private ObjectiveDefinitionSO _obtainAnimalObjective;

        private PlayerRuntimeDataSO _playerData;
        private GameRuntimeDataSO _gameData;

        private void Start()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerData);
            ScriptableObjectContainer.AssignIfNull(ref _gameData);
        }

        private void Update()
        {
            SyncMoveAnim(Agent.velocity.magnitude, Agent.speed);
        }

        private void SyncMoveAnim(float currentSpeed, float walkSpeed)
        {
            var lerpedSpeed = MathUtils.UnclampedInverseLerp(0, walkSpeed, currentSpeed);
            _animator.SetFloat(_moveSpeedParam, lerpedSpeed);
        }

        public void SetDefinition(AnimalDefinitionSO definition) => Definition = definition;

        public async UniTask Interact(object source, CancellationToken ct = default)
        {
            if (source is not PlayerController player) { return; }
            if (IsCaptured) { return; }

            InteractionUI.Show();
            InteractionUI.CaptureBtn.text = $"Capture?\n{InteractionConfig.captureCosts.Select(c => c.GetIconAmountText()).JoinToString(" ")}";
            var isCanceled = await InteractionUI.CaptureBtn.OnClickAsync(ct).SuppressCancellationThrow();
            if (isCanceled)
            {
                InteractionUI.Hide();
                return;
            }

            var canClear = _playerData.ResourceData.HasEnoughResources(InteractionConfig.captureCosts);
            if (!canClear)
            {
                Debug.LogWarning($"Insufficient resources to capture animal: {name}", this);
                return;
            }

            if (!HasValidHabitat(out var habitat))
            {
                Debug.LogWarning($"No habitat to capture animal: {name}", this);
                return;
            }
            InteractionUI.Hide();

            _playerData.ResourceData.AddResources(InteractionConfig.captureCosts.Select(c => c.Invert()));

            InputManager.Enable_PlayerMovement(false);

            player.Animator.PlayInteractAnim();

            player.UI.ProgressBar.Show();
            await player.UI.ProgressBar.RunProgressNormalized(0, 1, InteractionConfig.captureTime);
            CaptureAnimal(habitat);

            await Tween.Scale(player.UI.ProgressBar.transform, Vector3.one * 1.2f, 0.2f, Ease.OutSine, 2, CycleMode.Rewind);
            player.UI.ProgressBar.Hide();

            await UniTask.WaitForSeconds(0.5f);

            InputManager.Enable_PlayerMovement(true);

        }

        public bool HasValidHabitat(out Habitat habitat)
        {
            habitat = null;

            var habitats = FindObjectsByType<Habitat>(FindObjectsSortMode.None);
            habitat = habitats.FirstOrDefault(h => !h.IsFull);
            return habitat != null;
        }

        public void CaptureAnimal(Habitat targetHabitat)
        {
            if (targetHabitat.AddAnimal(this))
            {
                IsCaptured = true;
                QuestRegistrySO.Instance.IncreaseObjective(_obtainAnimalObjective);
            }

        }
    }

    [Serializable]
    public struct AnimalInteractionConfig
    {
        public ResourceAmount[] captureCosts;
        public SerializableTimeSpan captureTime;
    }
}