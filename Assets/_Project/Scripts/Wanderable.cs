using System.Collections;
using GameDevKit;
using UnityEngine;
using UnityEngine.AI;

namespace ZooTycoon
{
    /// <summary>
    /// Drives a NavMeshAgent to wander randomly within a given Collider bounds.
    /// Attach to any GameObject that also has a NavMeshAgent.
    /// Call StartWandering / StopWandering to control behaviour.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Wanderable : MonoBehaviour
    {
        [Tooltip("How long the agent pauses at each destination before picking a new one.")]
        public FloatRange IdleTime = new(1f, 3f);

        [Tooltip("How many times the system tries to find a valid NavMesh point before giving up this cycle.")]
        [SerializeField] private int _maxSampleAttempts = 10;

        [Tooltip("Radius around the candidate point used when sampling the NavMesh.")]
        [SerializeField] private float _navMeshSampleRadius = 2f;

        public Area Area;

        private NavMeshAgent _agent;
        private Coroutine _wanderCoroutine;

        private void Awake() => _agent = GetComponent<NavMeshAgent>();

        public void StartWandering() => StartWandering(Area);
        public void StartWandering(Area area)
        {
            Area = area;
            _wanderCoroutine.Stop(this);
            _wanderCoroutine = StartCoroutine(WanderRoutine());
        }

        public void StopWandering()
        {
            if (_wanderCoroutine != null)
            {
                StopCoroutine(_wanderCoroutine);
                _wanderCoroutine = null;
            }

            if (_agent != null && _agent.isActiveAndEnabled && _agent.isOnNavMesh)
            {
                _agent.ResetPath();
            }
        }

        private IEnumerator WanderRoutine()
        {
            while (true)
            {
                if (TryGetNavMeshPoint(out Vector3 destination))
                {
                    _agent.SetDestination(destination);

                    // Wait until the agent arrives or gets stuck.
                    yield return YieldCollection.WaitUntil(() =>
                        !_agent.pathPending &&
                        _agent.remainingDistance <= _agent.stoppingDistance &&
                        (!_agent.hasPath || _agent.velocity.sqrMagnitude < 0.01f));
                }

                yield return YieldCollection.WaitForSeconds(IdleTime.GetRandom());
            }
        }

        private bool TryGetNavMeshPoint(out Vector3 result)
        {
            if (Area == null)
            {
                Debug.LogWarning("Area not defined!", this);
                result = default;
                return false;
            }

            for (int i = 0; i < _maxSampleAttempts; i++)
            {
                Vector3 candidate = Area.GetRandomPoint();
                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, _navMeshSampleRadius, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            result = transform.position;
            return false;
        }
    }
}
