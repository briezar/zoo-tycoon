using UnityEngine;

namespace ZooTycoon
{
    /// <summary>
    /// An axis-aligned rectangular area centred on the GameObject's position (XZ plane).
    /// Respects the transform's Y rotation for oriented rectangles.
    /// </summary>
    public class RectangleArea : Area
    {
        [SerializeField] private Vector2 _size = new(10f, 10f);

        public Vector2 Size => _size;

        // ── Area ─────────────────────────────────────────────────────────────

        public override bool Contains(Vector3 worldPoint)
        {
            // Transform point into local space so rotation is handled automatically.
            Vector3 local = transform.InverseTransformPoint(worldPoint);
            return Mathf.Abs(local.x) <= _size.x * 0.5f &&
                   Mathf.Abs(local.z) <= _size.y * 0.5f;
        }

        public override Vector3 GetRandomPoint()
        {
            Vector3 localPoint = new(
                Random.Range(-_size.x * 0.5f, _size.x * 0.5f),
                0f,
                Random.Range(-_size.y * 0.5f, _size.y * 0.5f));

            return transform.TransformPoint(localPoint);
        }

#if UNITY_EDITOR

        // ── Gizmos ───────────────────────────────────────────────────────────

        protected override void DrawGizmoFilled()
        {
            // Drawn via Handles in OnDrawGizmos below.
        }

        protected override void DrawGizmoWire()
        {
            Vector3 c = transform.position;
            Vector3 right = transform.right * _size.x * 0.5f;
            Vector3 fwd = transform.forward * _size.y * 0.5f;

            Vector3 tl = c - right + fwd;
            Vector3 tr = c + right + fwd;
            Vector3 br = c + right - fwd;
            Vector3 bl = c - right - fwd;

            Gizmos.DrawLine(tl, tr);
            Gizmos.DrawLine(tr, br);
            Gizmos.DrawLine(br, bl);
            Gizmos.DrawLine(bl, tl);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Vector3 c = transform.position;
            Vector3 right = transform.right * _size.x * 0.5f;
            Vector3 fwd = transform.forward * _size.y * 0.5f;

            Vector3[] verts =
            {
                c - right + fwd,
                c + right + fwd,
                c + right - fwd,
                c - right - fwd,
            };

            UnityEditor.Handles.color = new Color(0.2f, 0.9f, 0.4f, 0.15f);
            UnityEditor.Handles.DrawAAConvexPolygon(verts);
        }
#endif

    }
}
