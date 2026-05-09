using GameDevKit;
using UnityEngine;

namespace ZooTycoon
{
    public class Debris : MonoBehaviour
    {
        [field: SerializeField] public IntAmount<ResourceSO> Cost { get; private set; }
        [field: SerializeField] public SerializableTimeSpan ClearTime { get; private set; }
    }
}