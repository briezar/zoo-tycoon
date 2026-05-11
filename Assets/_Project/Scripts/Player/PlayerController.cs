using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameDevKit;
using UnityEngine;
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
            var component = collider.attachedRigidbody == null ? (Component)collider : collider.attachedRigidbody;
            if (component.TryGetComponent<IInteractable>(out var interactable))
            {                
                Debug.Log($"Interacting with: {interactable.GetType().Name}");

                _interactionCts = new();
                await interactable.Interact(this, _interactionCts.Token);

                _interactionCts.Dispose();
                _interactionCts = null;
            }
        }
    }
}