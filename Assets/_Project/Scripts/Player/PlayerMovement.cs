using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using GameDevKit;
using GameDevKit.Pool;
using PrimeTween;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using ZooTycoon.AI;
using ZooTycoon.Input;

namespace ZooTycoon
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private AgentLinkMover _linkMover;
        [SerializeField] private LayerMask _groundLayer, _interactionLayer;
        [SerializeField] private PlayerAnimator _animator;
        [SerializeField] private SmartComponentPool<Transform> _markerPool;

        [Min(0)]
        public float WalkSpeed = 9f;

        [Min(0)]
        public float RunSpeed = 15f;

        [Tooltip("How close to the target should it be to count as target reached. Useful in cases where destination is blocked.")]
        [Min(0)]
        public float NearDestinationDistance = 1f;

        public readonly SourcedAction<Vector3> OnSetDestination = new();
        public readonly SourcedAction<Collider> OnTargetReached = new();

        public NavMeshAgent Agent => _agent;
        public float CurrentSpeed => _agent.velocity.magnitude;
        public float MaxSpeed
        {
            get => _agent.speed;
            set => _agent.speed = value;
        }

        private Coroutine _moveAnimSyncCoroutine, _trackTargetReachedCoroutine;
        private Tween _rotationTween;

        private void OnEnable()
        {
            InputManager.Enable_PlayerMovement(true);
            StartMoveAnimSync();
        }

        private void OnDisable()
        {
            InputManager.Enable_PlayerMovement(false);
        }

        private void Start()
        {
            InputManager.PlayerMovement.Move_Click.performed += HandleOnClick;
            InputManager.PlayerMovement.Move_DoubleClick.performed += HandleOnDoubleClick;
            InputManager.PlayerMovement.Move_ClickHold.performed += HandleOnClickHold;
            _linkMover.OnLinkStart.AddListener(HandleOnOffMeshLinkStart);
            _linkMover.OnLinkEnd.AddListener(HandleOnOffMeshLinkEnd);
        }

        private void HandleOnClick(InputAction.CallbackContext context)
        {
            var mousePos = Pointer.current.position.ReadValue();
            TryInteractAtMousePos(mousePos, out _);
            MaxSpeed = WalkSpeed;
        }

        private void HandleOnDoubleClick(InputAction.CallbackContext context) => MaxSpeed = RunSpeed;

        private void HandleOnClickHold(InputAction.CallbackContext context)
        {
            async UniTask RunTask()
            {
                while (InputManager.PlayerMovement.Move_ClickHold.IsPressed())
                {
                    var mousePos = Pointer.current.position.ReadValue();
                    TryMoveToMousePos(mousePos, out _);
                    await UniTask.WaitForSeconds(0.1f);
                }
            }
            RunTask();
        }

        public void StartMoveAnimSync()
        {
            _moveAnimSyncCoroutine.Stop(this);
            _moveAnimSyncCoroutine = StartCoroutine(SyncMovementAnimRoutine());
            IEnumerator SyncMovementAnimRoutine()
            {
                while (true)
                {
                    yield return YieldCollection.WaitForSeconds(0.1f);
                    _animator.SyncMoveAnim(CurrentSpeed, WalkSpeed);
                }
            }
        }

        public bool TryMoveToMousePos(Vector2 mousePos, out Vector3 targetPos)
        {
            targetPos = default;
            if (InputManager.IsPointerOverUI()) { return false; }

            _rotationTween.Stop();

            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundLayer))
            {
                targetPos = hit.point;
                SetDestination(targetPos);
                return true;
            }

            return false;
        }

        public bool TryInteractAtMousePos(Vector2 mousePos, out Vector3 targetPos)
        {
            targetPos = default;
            if (InputManager.IsPointerOverUI()) { return false; }

            _rotationTween.Stop();
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _interactionLayer))
            {
                Debug.Log($"Clicked: {hit.collider.name}");
                if (_groundLayer.Contains(hit.collider.gameObject.layer))
                {
                    targetPos = hit.point;
                }
                else
                {
                    targetPos = hit.collider.ClosestPointOnBounds(transform.position).With(y: 0);
                }

                _markerPool.GetAndAutoPool(targetPos);

                SetDestination(targetPos);
                TrackTargetReached(hit.collider);
                return true;
            }

            return false;
        }

        public void SetDestination(Vector3 worldPos)
        {
            _trackTargetReachedCoroutine.Stop(this);
            OnSetDestination?.Invoke(worldPos);
            _agent.SetDestination(worldPos);
        }

        private void TrackTargetReached(Collider collider)
        {
            _trackTargetReachedCoroutine.Stop(this);
            _trackTargetReachedCoroutine = StartCoroutine(TrackRoutine());

            IEnumerator TrackRoutine()
            {
                yield return null;

                var isNearDestination = false;

                while (true)
                {
                    yield return YieldCollection.WaitUntil(() => !_agent.pathPending);

                    var reachedDestination = _agent.remainingDistance <= _agent.stoppingDistance + 0.01f;
                    isNearDestination = _agent.remainingDistance <= _agent.stoppingDistance + NearDestinationDistance;

                    var stoppedMoving = !_agent.hasPath || _agent.velocity.sqrMagnitude <= 0.01f;

                    if (reachedDestination) { break; }
                    if (stoppedMoving && isNearDestination) { break; }

                    yield return null;
                }

                _agent.ResetPath();

                var direction = (collider.transform.position - transform.position).normalized.With(y: 0);
                if (direction != Vector3.zero && !_groundLayer.Contains(collider.gameObject.layer))
                {
                    _rotationTween = Tween.Rotation(_agent.transform, Quaternion.LookRotation(direction), 0.5f);
                }

                if (isNearDestination)
                {
                    OnTargetReached?.Invoke(collider);
                }
            }
        }

        private void HandleOnOffMeshLinkStart(OffMeshLinkMoveMode moveMode)
        {
            _moveAnimSyncCoroutine.Stop(this);
            _animator.SyncMoveAnim(CurrentSpeed, WalkSpeed);
        }

        private void HandleOnOffMeshLinkEnd(OffMeshLinkMoveMode moveMode) => StartMoveAnimSync();

    }
}