using UnityEngine;

namespace ZooTycoon
{
    /// <summary>
    /// A circular area centred on the GameObject's position (XZ plane).
    /// </summary>
    public class CircleArea : Area
    {
        [SerializeField] private float _radius = 5f;

        public float Radius => _radius;

        // ── Area ─────────────────────────────────────────────────────────────

        public override bool Contains(Vector3 worldPoint)
        {
            Vector3 centre = transform.position;
            float dx = worldPoint.x - centre.x;
            float dz = worldPoint.z - centre.z;
            return dx * dx + dz * dz <= _radius * _radius;
        }

        public override Vector3 GetRandomPoint()
        {
            // Uniform disk sampling: sqrt ensures even distribution (not polar-clumped).
            float r = _radius * Mathf.Sqrt(Random.value);
            float angle = Random.value * Mathf.PI * 2f;
            Vector3 centre = transform.position;
            return new Vector3(
                centre.x + r * Mathf.Cos(angle),
                centre.y,
                centre.z + r * Mathf.Sin(angle));
        }

#if UNITY_EDITOR

        // ── Gizmos ───────────────────────────────────────────────────────────

        protected override void DrawGizmoFilled()
        {
            // Unity has no built-in filled circle gizmo; approximate with a disc mesh via Handles.
            // Filled colour is handled by DrawGizmoWire fill trick using a flat cylinder approach.
            // We rely on the wire outline only for fill illusion at editor scale.
        }

        protected override void DrawGizmoWire()
        {
            Vector3 centre = transform.position;
            int segments = 48;
            float step = Mathf.PI * 2f / segments;
            Vector3 prev = centre + new Vector3(_radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float a = i * step;
                Vector3 next = centre + new Vector3(Mathf.Cos(a) * _radius, 0f, Mathf.Sin(a) * _radius);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        protected override void OnDrawGizmos()
        {
            // Use Handles for a proper filled disc in the editor.
            base.OnDrawGizmos();
            UnityEditor.Handles.color = new Color(0.2f, 0.9f, 0.4f, 0.15f);
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, _radius);
        }
#endif

    }
}
