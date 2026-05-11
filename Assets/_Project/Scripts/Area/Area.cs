using UnityEngine;
using EditorAttributes;

namespace ZooTycoon
{
    /// <summary>
    /// Abstract base for editor-visible 2D areas (on the XZ plane).
    /// Derive to define shape-specific containment and random point sampling.
    /// </summary>
    public abstract class Area : MonoBehaviour
    {
        private const float DEBUG_POINT_DURATION = 2f;
        private const float DEBUG_POINT_RADIUS = 0.25f;

        private Vector3 _debugPoint;
        private float _debugPointExpiry = -1f;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Returns true if the given world-space point lies inside this area (XZ plane).</summary>
        public abstract bool Contains(Vector3 worldPoint);

        /// <summary>Returns a uniformly random world-space point inside this area.</summary>
        public abstract Vector3 GetRandomPoint();

#if UNITY_EDITOR

        // ── Debug ─────────────────────────────────────────────────────────────

        [Button()]
        private void DebugGetRandomPoint()
        {
            _debugPoint = GetRandomPoint();
            _debugPointExpiry = (float)UnityEditor.EditorApplication.timeSinceStartup + DEBUG_POINT_DURATION;
        }

        // ── Gizmos ────────────────────────────────────────────────────────────

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.25f);
            DrawGizmoFilled();
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.9f);
            DrawGizmoWire();
            DrawDebugPoint();
        }

        private void DrawDebugPoint()
        {
            if (_debugPointExpiry < 0f) return;

            bool expired = UnityEditor.EditorApplication.timeSinceStartup > _debugPointExpiry;
            if (expired) { _debugPointExpiry = -1f; return; }

            float t = 1f - (float)(_debugPointExpiry - UnityEditor.EditorApplication.timeSinceStartup) / DEBUG_POINT_DURATION;
            Gizmos.color = Color.Lerp(Color.yellow, new Color(1f, 1f, 0f, 0f), t);
            Gizmos.DrawSphere(_debugPoint, DEBUG_POINT_RADIUS);
            Gizmos.color = Color.Lerp(Color.red, new Color(1f, 0f, 0f, 0f), t);
            Gizmos.DrawWireSphere(_debugPoint, DEBUG_POINT_RADIUS);
        }

        /// <summary>Draw a filled gizmo representation of this area.</summary>
        protected abstract void DrawGizmoFilled();

        /// <summary>Draw a wire gizmo representation of this area.</summary>
        protected abstract void DrawGizmoWire();

#endif

    }
}