using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameDevKit;
using PrimeTween;
using UnityEngine;
using ZooTycoon.Input;
using ZooTycoon.RuntimeData;

namespace ZooTycoon
{
    public class Debris : AdvancedBehaviour, IInteractable
    {
        [field: SerializeField] public DebrisInteractionConfig InteractionConfig { get; private set; }

        [field: SerializeField] public DebrisInteractionUI InteractionUI { get; private set; }

        [SerializeField] private GameObject _clearFx;

        private GameRuntimeDataSO _gameData;
        private PlayerRuntimeDataSO _playerData;

        protected override void OnStart()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerData);
            ScriptableObjectContainer.AssignIfNull(ref _gameData);
        }

        public async UniTask Interact(object source, CancellationToken ct = default)
        {
            if (source is not PlayerController player) { return; }

            InteractionUI.Show();
            InteractionUI.ClearDebrisBtn.text = $"Clear?\n{InteractionConfig.clearCosts.Select(c => c.GetIconAmountText()).JoinToString(" ")}";
            var isCanceled = await InteractionUI.ClearDebrisBtn.OnClickAsync(ct).SuppressCancellationThrow();
            if (isCanceled)
            {
                InteractionUI.Hide();
                return;
            }

            var canClear = _playerData.ResourceData.HasEnoughResources(InteractionConfig.clearCosts);
            if (!canClear)
            {
                Debug.Log($"Insufficient resources to clear debris: {name}", this);
                return;
            }
            InteractionUI.Hide();

            _playerData.ResourceData.AddResources(InteractionConfig.clearCosts.Select(c => c.Invert()));

            InputManager.Enable_PlayerMovement(false);

            player.Animator.PlayAttackAnim();

            player.UI.ProgressBar.Show();
            await player.UI.ProgressBar.RunProgressNormalized(0, 1, InteractionConfig.clearTime);
            ClearDebris();

            _playerData.ResourceData.AddResources(InteractionConfig.clearRewards);
            _gameData.TotalDebrisCleared.Value++;

            await Tween.Scale(player.UI.ProgressBar.transform, Vector3.one * 1.2f, 0.2f, Ease.OutSine, 2, CycleMode.Rewind);
            player.UI.ProgressBar.Hide();

            await UniTask.WaitForSeconds(0.5f);
            InputManager.Enable_PlayerMovement(true);
        }

        public void ClearDebris()
        {
            Instantiate(_clearFx, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    [Serializable]
    public struct DebrisInteractionConfig
    {
        public ResourceAmount[] clearCosts;
        public SerializableTimeSpan clearTime;
        public ResourceAmount[] clearRewards;
    }
}