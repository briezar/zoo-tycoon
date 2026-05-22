using UnityEngine;

namespace ZooTycoon
{
    /// <summary>
    /// A circular area centred on the GameObject's position.
    /// Plane is configured via <see cref="Area2D.PlaneNormal"/> (default: XZ / Vector3.up).
    /// </summary>
    public class CircleArea2D : Area2D
    {
        public float Radius = 5f;

        public override bool Contains(Vector3 worldPoint)
        {
            var uv = WorldToPlane(worldPoint);
            return uv.sqrMagnitude <= Radius * Radius;
        }

        public override Vector3 GetRandomPoint()
        {
            // Uniform disk sampling: sqrt ensures even distribution (not polar-clumped).
            var r = Radius * Mathf.Sqrt(Random.value);
            var angle = Random.value * Mathf.PI * 2f;
            return PlaneToWorld(new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle)));
        }

#if UNITY_EDITOR

        protected override void DrawGizmoWire()
        {
            var centre = transform.position;
            var segments = 48;
            var step = Mathf.PI * 2f / segments;
            var prev = centre + PlaneRight * Radius;

            for (int i = 1; i <= segments; i++)
            {
                var a = i * step;
                var next = centre + PlaneRight * (Mathf.Cos(a) * Radius) + PlaneForward * (Mathf.Sin(a) * Radius);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            UnityEditor.Handles.color = new Color(0.2f, 0.9f, 0.4f, 0.15f);
            UnityEditor.Handles.DrawSolidDisc(transform.position, PlaneNormal, Radius);
        }
#endif
    }
}