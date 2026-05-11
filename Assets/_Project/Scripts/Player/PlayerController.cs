using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameDevKit;
using PrimeTween;
using UnityEngine;
using ZooTycoon.Input;
using ZooTycoon.QuestSystem;
using ZooTycoon.RuntimeData;

namespace ZooTycoon
{
    public class PlayerController : AdvancedBehaviour
    {
        [field: SerializeField] public PlayerAnimator Animator { get; private set; }
        [field: SerializeField] public PlayerMovement Movement { get; private set; }
        [field: SerializeField] public PlayerWorldUI UI { get; private set; }

        [Header("Optional")]
        [SerializeField] private PlayerRuntimeDataSO _playerData;
        [SerializeField] private GameRuntimeDataSO _gameData;

        private CancellationTokenSource _interactionCts;

        protected override void OnStart()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerData);
            ScriptableObjectContainer.AssignIfNull(ref _gameData);
            Movement.OnSetDestination[this] += (p) => _interactionCts?.Cancel();
            Movement.OnTargetReached[this] += (c) => HandleOnTargetReached(c);
        }

        private async UniTask HandleOnTargetReached(Collider collider)
        {
            Component component = collider.attachedRigidbody == null ? collider : collider.attachedRigidbody;
            if (component.TryGetComponent<Debris>(out var debris))
            {
                _interactionCts = new();

                await debris.Interact(_interactionCts.Token);
                if (_interactionCts.IsCancellationRequested)
                {
                    return;
                }

                _interactionCts.Dispose();
                _interactionCts = null;

                var canClear = _playerData.ResourceData.HasEnoughResources(debris.Config.clearCosts);
                if (!canClear)
                {
                    Debug.Log($"Insufficient resources to clear debris {debris}");
                    return;
                }

                _playerData.ResourceData.AddResources(debris.Config.clearCosts.Select(c => c.Invert()));

                InputManager.Enable_PlayerMovement(false);

                debris.InteractionUI.Hide();
                UI.ProgressBar.Show();
                await UI.ProgressBar.RunProgressNormalized(0, 1, debris.Config.clearTime);
                debris.Clear();

                _playerData.ResourceData.AddResources(debris.Config.clearRewards);
                _gameData.TotalDebrisCleared.Value++;

                await Tween.Scale(UI.ProgressBar.transform, Vector3.one * 1.2f, 0.2f, Ease.OutSine, 2, CycleMode.Rewind);
                UI.ProgressBar.Hide();

                await UniTask.WaitForSeconds(0.5f);
                InputManager.Enable_PlayerMovement(true);
            }
        }

    }
}