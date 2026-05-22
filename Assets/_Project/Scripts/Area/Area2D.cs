using UnityEngine;
using EditorAttributes;

namespace ZooTycoon
{
    public interface IArea2D
    {
        /// <summary>Returns true if the given world-space point lies inside this area.</summary>
        bool Contains(Vector3 worldPoint);

        /// <summary>Returns a uniformly random world-space point inside this area.</summary>
        Vector3 GetRandomPoint();
    }

    /// <summary>
    /// Abstract base for editor-visible areas on a configurable plane.
    /// The plane is defined by <see cref="PlaneNormal"/> (default Vector3.up = XZ plane).
    /// For 2D games on the XY plane, set PlaneNormal to Vector3.forward.
    /// </summary>
    public abstract class Area2D : MonoBehaviour, IArea2D
    {
        [Tooltip("Defines which direction is UP. (0,1,0) for XZ plane (3D), (0,0,1) for XY plane (2D).")]
        public Vector3 PlaneNormal = Vector3.up;

        /// <summary>One axis of the sampling plane, derived from PlaneNormal.</summary>
        public Vector3 PlaneRight => Vector3.Cross(PlaneNormal, GetPerp(PlaneNormal)).normalized;

        /// <summary>Other axis of the sampling plane, derived from PlaneNormal.</summary>
        public Vector3 PlaneForward => Vector3.Cross(PlaneRight, PlaneNormal).normalized;

        public abstract bool Contains(Vector3 worldPoint);

        public abstract Vector3 GetRandomPoint();

        /// <summary>
        /// Projects a world-space point onto the plane as a local 2D coordinate (u, v).
        /// Used by subclasses instead of InverseTransformPoint to respect PlaneNormal.
        /// </summary>
        protected Vector2 WorldToPlane(Vector3 worldPoint)
        {
            var delta = worldPoint - transform.position;
            return new Vector2(
                Vector3.Dot(delta, PlaneRight),
                Vector3.Dot(delta, PlaneForward));
        }

        /// <summary>
        /// Converts a local plane coordinate (u, v) back to a world-space point on the plane.
        /// </summary>
        protected Vector3 PlaneToWorld(Vector2 planePoint) => transform.position + PlaneRight * planePoint.x + PlaneForward * planePoint.y;

        // Returns a vector that is not parallel to n, used to derive a stable tangent.
        private static Vector3 GetPerp(Vector3 n) => Mathf.Abs(n.x) < 0.9f ? Vector3.right : Vector3.up;

#if UNITY_EDITOR

        // ── Debug ─────────────────────────────────────────────────────────────

        private const float DEBUG_POINT_DURATION = 2f;
        private const float DEBUG_POINT_RADIUS = 0.25f;

        private Vector3 _debugPoint;
        private float _debugPointExpiry = -1f;

        [Button]
        private void DebugGetRandomPoint()
        {
            _debugPoint = GetRandomPoint();
            _debugPointExpiry = (float)UnityEditor.EditorApplication.timeSinceStartup + DEBUG_POINT_DURATION;
        }

        // ── Gizmos ────────────────────────────────────────────────────────────

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.25f);
            DrawGizmoFilled();
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.9f);
            DrawGizmoWire();
            DrawDebugPoint();
        }

        private void DrawDebugPoint()
        {
            if (_debugPointExpiry < 0f) { return; }

            var expired = UnityEditor.EditorApplication.timeSinceStartup > _debugPointExpiry;
            if (expired) { _debugPointExpiry = -1f; return; }

            var t = 1f - (float)(_debugPointExpiry - UnityEditor.EditorApplication.timeSinceStartup) / DEBUG_POINT_DURATION;
            Gizmos.color = Color.Lerp(Color.yellow, new Color(1f, 1f, 0f, 0f), t);
            Gizmos.DrawSphere(_debugPoint, DEBUG_POINT_RADIUS);
            Gizmos.color = Color.Lerp(Color.red, new Color(1f, 0f, 0f, 0f), t);
            Gizmos.DrawWireSphere(_debugPoint, DEBUG_POINT_RADIUS);
        }

        /// <summary>Draw a filled gizmo representation of this area.</summary>
        protected virtual void DrawGizmoFilled() { }

        /// <summary>Draw a wire gizmo representation of this area.</summary>
        protected virtual void DrawGizmoWire() { }

#endif
    }
}