using UnityEngine;

namespace ZooTycoon
{
    /// <summary>
    /// Abstract base for editor-visible 2D areas (on the XZ plane).
    /// Derive to define shape-specific containment and random point sampling.
    /// </summary>
    public abstract class Area : MonoBehaviour
    {
        /// <summary>Returns true if the given world-space point lies inside this area (XZ plane).</summary>
        public abstract bool Contains(Vector3 worldPoint);

        /// <summary>Returns a uniformly random world-space point inside this area.</summary>
        public abstract Vector3 GetRandomPoint();

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.25f);
            DrawGizmoFilled();
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.9f);
            DrawGizmoWire();
        }

        /// <summary>Draw a filled gizmo representation of this area.</summary>
        protected abstract void DrawGizmoFilled();

        /// <summary>Draw a wire gizmo representation of this area.</summary>
        protected abstract void DrawGizmoWire();
    }
}
