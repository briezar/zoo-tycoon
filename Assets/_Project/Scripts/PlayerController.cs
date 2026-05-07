using System.Collections;
using Cysharp.Threading.Tasks;
using EditorAttributes;
using GameDevKit;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace ZooTycoon
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private Animator _animator;

        public float WalkSpeed = 9f;
        public float RunSpeed = 15f;

        public float CurrentSpeed => _agent.velocity.magnitude;

        private readonly AnimationHash _moveSpeedHash = "Move Speed";

        private void OnEnable()
        {
            InputManager.PlayerMovement.Enable();
            StartCoroutine(RunAnimationUpdateRoutine());
        }

        private void OnDisable()
        {
            if (InputManager.IsValid)
            {
                InputManager.PlayerMovement.Disable();
            }
        }

        private void Start()
        {
            InputManager.PlayerMovement.Move_Click.performed += HandleOnClick;
            InputManager.PlayerMovement.Move_DoubleClick.performed += HandleOnDoubleClick;
            InputManager.PlayerMovement.Move_ClickHold.performed += HandleOnClickHold;
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

        private IEnumerator RunAnimationUpdateRoutine()
        {
            while (true)
            {
                yield return YieldCollection.WaitForSeconds(0.1f);
                var currentSpeed = CurrentSpeed;
                var lerpedSpeed = UnclampedInverseLerp(0, WalkSpeed, currentSpeed);
                _animator.SetFloat(_moveSpeedHash, lerpedSpeed);
            }
        }

        private bool TryMoveToMousePos(Vector2 mousePos, out Vector3 targetPos)
        {
            targetPos = default;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundLayer))
            {
                NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1f, NavMesh.AllAreas);
                targetPos = navHit.position;
                _agent.SetDestination(targetPos);
                return true;
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