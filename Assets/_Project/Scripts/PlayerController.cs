using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using EditorAttributes;
using GameDevKit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using ZooTycoon.AI;
using ZooTycoon.Input;

namespace ZooTycoon
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private Animator _animator;
        [SerializeField] private AgentLinkMover _linkMover;

        public float WalkSpeed = 9f;
        public float RunSpeed = 15f;

        public float CurrentSpeed => _agent.velocity.magnitude;

        private readonly AnimationHash _moveSpeedHash = "Move Speed";

        private Coroutine _animationUpdateRoutine;

        private void OnEnable()
        {
            InputManager.Enable_PlayerMovement(true);
            StartSyncMovementAnimRoutine();
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
            TryMoveToMousePos(mousePos, out _);
            _agent.speed = WalkSpeed;
        }

        private void HandleOnDoubleClick(InputAction.CallbackContext context) => _agent.speed = RunSpeed;

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

        private void StartSyncMovementAnimRoutine()
        {
            _animationUpdateRoutine.Stop(this);
            _animationUpdateRoutine = StartCoroutine(SyncMovementAnimRoutine());
            IEnumerator SyncMovementAnimRoutine()
            {
                while (true)
                {
                    yield return YieldCollection.WaitForSeconds(0.1f);
                    var currentSpeed = CurrentSpeed;
                    var lerpedSpeed = UnclampedInverseLerp(0, WalkSpeed, currentSpeed);
                    _animator.SetFloat(_moveSpeedHash, lerpedSpeed);
                }
            }
        }

        private void HandleOnOffMeshLinkStart(OffMeshLinkMoveMode moveMode)
        {
            _animationUpdateRoutine.Stop(this);

            // Default to walking if not running (> 1f)
            _animator.SetFloat(_moveSpeedHash, Mathf.Min(_animator.GetFloat(_moveSpeedHash), 0.1f));
        }

        private void HandleOnOffMeshLinkEnd(OffMeshLinkMoveMode moveMode) => StartSyncMovementAnimRoutine();

        private bool TryMoveToMousePos(Vector2 mousePos, out Vector3 targetPos)
        {
            targetPos = default;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundLayer))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
                {
                    targetPos = navHit.position;
                    _agent.SetDestination(targetPos);
                    return true;
                }
            }

            return false;
        }

        /// <summary> Mathf.InverseLerp clamps between 0-1. This method can return values outside 0-1. </summary>
        private static float UnclampedInverseLerp(float a, float b, float value) => (value - a) / (b - a);

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _animator.Play("attack-melee-right", 1);
            }
        }
    }
}