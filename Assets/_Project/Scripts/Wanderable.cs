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
        public FloatRange IdleTime = new(2f, 5f);

        [Tooltip("How many times the system tries to find a valid NavMesh point before giving up this cycle.")]
        [SerializeField] private int _maxSampleAttempts = 10;

        [Tooltip("Radius around the candidate point used when sampling the NavMesh.")]
        [SerializeField] private float _navMeshSampleRadius = 2f;

        public Area Area;

        public float NearDestinationDistance = 1f;
        public bool WanderOnEnable;

        private NavMeshAgent _agent;
        private Coroutine _wanderCoroutine;

        private void Awake() => _agent = GetComponent<NavMeshAgent>();

        private void OnEnable()
        {
            if (WanderOnEnable)
            {
                StartWandering();
            }
        }

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
