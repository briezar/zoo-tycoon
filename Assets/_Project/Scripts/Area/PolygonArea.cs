using System.Collections.Generic;
using UnityEngine;

namespace ZooTycoon
{
    /// <summary>
    /// A convex or concave polygon area defined by local-space XZ vertices.
    /// Containment uses a ray-casting point-in-polygon test (works for any simple polygon).
    /// Random point sampling uses rejection sampling inside the AABB.
    /// </summary>
    public class PolygonArea : Area
    {
        [Tooltip("Local-space XZ vertices defining the polygon. Y is ignored.")]
        [SerializeField]
        private List<Vector2> _vertices = new()
        {
            new(-5f,  5f),
            new( 5f,  5f),
            new( 5f, -5f),
            new(-5f, -5f),
        };

        [Tooltip("Max rejection-sampling attempts before returning the centroid as fallback.")]
        [SerializeField] private int _maxSampleAttempts = 50;

        // ── Area ─────────────────────────────────────────────────────────────

        public override bool Contains(Vector3 worldPoint)
        {
            Vector3 local = transform.InverseTransformPoint(worldPoint);
            return PointInPolygon(new Vector2(local.x, local.z));
        }

        public override Vector3 GetRandomPoint()
        {
            Bounds2D aabb = GetLocalAABB();

            for (int i = 0; i < _maxSampleAttempts; i++)
            {
                Vector2 candidate = new(
                    Random.Range(aabb.min.x, aabb.max.x),
                    Random.Range(aabb.min.y, aabb.max.y));

                if (PointInPolygon(candidate))
                    return transform.TransformPoint(new Vector3(candidate.x, 0f, candidate.y));
            }

            // Fallback: return centroid.
            return transform.TransformPoint(Vector3.zero);
        }

        // ── Geometry helpers ─────────────────────────────────────────────────

        /// <summary>Ray-casting point-in-polygon test (Jordan curve theorem).</summary>
        private bool PointInPolygon(Vector2 point)
        {
            int count = _vertices.Count;
            if (count < 3) return false;

            bool inside = false;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                Vector2 vi = _vertices[i];
                Vector2 vj = _vertices[j];

                bool crosses = (vi.y > point.y) != (vj.y > point.y) &&
                               point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x;
                if (crosses) inside = !inside;
            }
            return inside;
        }

        private Bounds2D GetLocalAABB()
        {
            Vector2 min = new(float.MaxValue, float.MaxValue);
            Vector2 max = new(float.MinValue, float.MinValue);
            foreach (Vector2 v in _vertices)
            {
                if (v.x < min.x) min.x = v.x;
                if (v.y < min.y) min.y = v.y;
                if (v.x > max.x) max.x = v.x;
                if (v.y > max.y) max.y = v.y;
            }
            return new Bounds2D(min, max);
        }

        private readonly struct Bounds2D
        {
            public readonly Vector2 min;
            public readonly Vector2 max;
            public Bounds2D(Vector2 min, Vector2 max) { this.min = min; this.max = max; }
        }

#if UNITY_EDITOR

        // ── Gizmos ───────────────────────────────────────────────────────────

        protected override void DrawGizmoFilled() { /* Handled in OnDrawGizmos */ }

        protected override void DrawGizmoWire()
        {
            int count = _vertices.Count;
            if (count < 2) return;

            for (int i = 0; i < count; i++)
            {
                Vector2 a = _vertices[i];
                Vector2 b = _vertices[(i + 1) % count];
                Vector3 wa = transform.TransformPoint(new Vector3(a.x, 0f, a.y));
                Vector3 wb = transform.TransformPoint(new Vector3(b.x, 0f, b.y));
                Gizmos.DrawLine(wa, wb);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            int count = _vertices.Count;
            if (count < 3) return;

            Vector3[] worldVerts = new Vector3[count];
            for (int i = 0; i < count; i++)
                worldVerts[i] = transform.TransformPoint(new Vector3(_vertices[i].x, 0f, _vertices[i].y));

            UnityEditor.Handles.color = new Color(0.2f, 0.9f, 0.4f, 0.15f);
            UnityEditor.Handles.DrawAAConvexPolygon(worldVerts); // Fill (convex only)

            // Always draw the wire so concave shapes are clearly outlined.
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.9f);
            DrawGizmoWire();
        }
#endif

    }
}
