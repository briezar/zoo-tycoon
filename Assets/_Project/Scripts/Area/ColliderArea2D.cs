using UnityEngine;

namespace ZooTycoon
{
    public class ColliderArea2D : Area2D
    {
        public Collider Collider;

        public override bool Contains(Vector3 worldPoint) => Collider.Contains(worldPoint);
        public override Vector3 GetRandomPoint()
        {
            var point = Collider.GetRandomPoint();
            return point - Vector3.Scale(point, PlaneNormal);
        }

    }
}