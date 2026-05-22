using System.Buffers;
using UnityEngine;

namespace ZooTycoon
{
    /// <summary>
    /// A rectangular area centred on the GameObject's position.
    /// Plane is configured via <see cref="Area2D.PlaneNormal"/> (default: XZ / Vector3.up).
    /// </summary>
    public class RectangleArea2D : Area2D
    {
        public Vector2 Size = new(10f, 10f);


        public override bool Contains(Vector3 worldPoint)
        {
            var uv = WorldToPlane(worldPoint);
            return Mathf.Abs(uv.x) <= Size.x * 0.5f && Mathf.Abs(uv.y) <= Size.y * 0.5f;
        }

        public override Vector3 GetRandomPoint()
        {
            return PlaneToWorld(new Vector2(
                Random.Range(-Size.x * 0.5f, Size.x * 0.5f),
                Random.Range(-Size.y * 0.5f, Size.y * 0.5f)));
        }

#if UNITY_EDITOR

        protected override void DrawGizmoWire()
        {
            var c = transform.position;
            var r = PlaneRight * (Size.x * 0.5f);
            var fwd = PlaneForward * (Size.y * 0.5f);

            var tl = c - r + fwd;
            var tr = c + r + fwd;
            var br = c + r - fwd;
            var bl = c - r - fwd;

            Gizmos.DrawLine(tl, tr);
            Gizmos.DrawLine(tr, br);
            Gizmos.DrawLine(br, bl);
            Gizmos.DrawLine(bl, tl);
        }

        private readonly Vector3[] _verts = new Vector3[4];
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            var c = transform.position;
            var r = PlaneRight * (Size.x * 0.5f);
            var fwd = PlaneForward * (Size.y * 0.5f);

            _verts[0] = c - r + fwd;
            _verts[1] = c + r + fwd;
            _verts[2] = c + r - fwd;
            _verts[3] = c - r - fwd;

            UnityEditor.Handles.color = new Color(0.2f, 0.9f, 0.4f, 0.15f);
            UnityEditor.Handles.DrawAAConvexPolygon(_verts);
        }
#endif
    }
}