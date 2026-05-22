using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace ZooTycoon
{
    /// <summary>
    /// A polygon area defined by local-space UV vertices on a configurable plane.
    /// Containment uses a ray-casting point-in-polygon test (works for any simple polygon).
    /// Random point sampling uses rejection sampling inside the AABB.
    /// Plane is configured via <see cref="Area2D.PlaneNormal"/> (default: XZ / Vector3.up).
    /// </summary>
    public class PolygonArea2D : Area2D
    {
        [Tooltip("Local-space plane-UV vertices defining the polygon.")]
        public List<Vector2> Vertices = new()
        {
            new(-5f,  5f),
            new( 5f,  5f),
            new( 5f, -5f),
            new(-5f, -5f),
        };

        [Tooltip("Max rejection-sampling attempts before returning the origin as fallback.")]
        public int MaxSampleAttempts = 50;


        public override bool Contains(Vector3 worldPoint) => PointInPolygon(WorldToPlane(worldPoint));

        public override Vector3 GetRandomPoint()
        {
            var (min, max) = GetAABB();

            for (int i = 0; i < MaxSampleAttempts; i++)
            {
                var candidate = new Vector2(
                    Random.Range(min.x, max.x),
                    Random.Range(min.y, max.y));

                if (PointInPolygon(candidate)) { return PlaneToWorld(candidate); }
            }

            return PlaneToWorld(Vector2.zero);
        }

        // ── Geometry helpers ─────────────────────────────────────────────────

        /// <summary>Ray-casting point-in-polygon test (Jordan curve theorem).</summary>
        private bool PointInPolygon(Vector2 point)
        {
            var count = Vertices.Count;
            if (count < 3) { return false; }

            var inside = false;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                var vi = Vertices[i];
                var vj = Vertices[j];
                bool crosses = (vi.y > point.y) != (vj.y > point.y) && point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x;
                if (crosses) { inside = !inside; }
            }
            return inside;
        }

        private (Vector2 min, Vector2 max) GetAABB()
        {
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            foreach (var v in Vertices)
            {
                if (v.x < min.x) { min.x = v.x; }
                if (v.y < min.y) { min.y = v.y; }
                if (v.x > max.x) { max.x = v.x; }
                if (v.y > max.y) { max.y = v.y; }
            }
            return (min, max);
        }

#if UNITY_EDITOR

        protected override void DrawGizmoWire()
        {
            var count = Vertices.Count;
            if (count < 2) { return; }

            for (int i = 0; i < count; i++)
            {
                var a = Vertices[i];
                var b = Vertices[(i + 1) % count];
                Gizmos.DrawLine(PlaneToWorld(a), PlaneToWorld(b));
            }
        }

        private readonly Vector3[] _verts = new Vector3[4];
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            var count = Vertices.Count;
            if (count < 3) { return; }

            for (int i = 0; i < count; i++) { _verts[i] = PlaneToWorld(Vertices[i]); }

            UnityEditor.Handles.color = new Color(0.2f, 0.9f, 0.4f, 0.15f);
            UnityEditor.Handles.DrawAAConvexPolygon(_verts);

            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.9f);
            DrawGizmoWire();
        }
#endif
    }
}