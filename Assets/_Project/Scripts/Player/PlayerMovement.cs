using System.Collections;
using Cysharp.Threading.Tasks;
using GameDevKit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using ZooTycoon.AI;
using ZooTycoon.Input;

namespace ZooTycoon
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private AgentLinkMover _linkMover;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private PlayerAnimator _animator;

        [Min(0)]
        public float WalkSpeed = 9f;

        [Min(0)]
        public float RunSpeed = 15f;

        public NavMeshAgent Agent => _agent;
        public float CurrentSpeed => _agent.velocity.magnitude;
        public float MaxSpeed
        {
            get => _agent.speed;
            set => _agent.speed = value;
        }

        private Coroutine _moveAnimSyncCoroutine;

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
            TryMoveToMousePos(mousePos, out _);
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

        private void HandleOnOffMeshLinkStart(OffMeshLinkMoveMode moveMode)
        {
            _moveAnimSyncCoroutine.Stop(this);
            _animator.SyncMoveAnim(CurrentSpeed, WalkSpeed);
        }

        private void HandleOnOffMeshLinkEnd(OffMeshLinkMoveMode moveMode) => StartMoveAnimSync();

    }
}