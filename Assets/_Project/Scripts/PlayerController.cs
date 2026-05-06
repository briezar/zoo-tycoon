using EditorAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace ZooTycoon
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private Animator _animator;

        [SerializeField] private InputActionReference _clickAction, _doubleClickAction;

        public float WalkSpeed = 9f;
        public float RunSpeed = 15f;

        public float CurrentSpeed => _agent.velocity.magnitude;

        private readonly int _moveSpeedHash = Animator.StringToHash("Move Speed");

        private void Update()
        {
            if (_clickAction.action.triggered)
            {
                var mousePos = Pointer.current.position.ReadValue();
                var ray = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out RaycastHit hit, _groundLayer))
                {
                    _agent.speed = WalkSpeed;
                    _agent.SetDestination(hit.point);
                }
            }

            if (_doubleClickAction.action.triggered)
            {
                _agent.speed = RunSpeed;
            }

            var currentSpeed = CurrentSpeed;
            var lerpedSpeed = UnclampedInverseLerp(0, WalkSpeed, currentSpeed);
            _animator.SetFloat(_moveSpeedHash, lerpedSpeed);

        }

        /// <summary> Mathf.InverseLerp clamps between 0-1. This method can return values outside 0-1. </summary>
        private static float UnclampedInverseLerp(float a, float b, float value) => (value - a) / (b - a);
    }
}