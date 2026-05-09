using System;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon
{
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/Resource")]
    public class ResourceSO : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }

        [field: AssetPreview(24, 24)]
        [field: SerializeField] public Sprite Icon { get; private set; }

    }

    [Serializable]
    public struct ResourceAmount
    {
        public ResourceSO resource;
        public int amount;

        public ResourceAmount(ResourceSO resource, int amount = 0) => (this.resource, this.amount) = (resource, amount);

        public static ResourceAmount operator +(ResourceAmount left, ResourceAmount right)
        {
            if (left.resource != right.resource)
            {
                Debug.LogError($"Cannot add ResourceAmount values with different resources: '{left.resource}' and '{right.resource}'.");
                return left;
            }

            return new(left.resource, left.amount + right.amount);
        }

        public static ResourceAmount operator -(ResourceAmount left, ResourceAmount right)
        {
            if (left.resource != right.resource)
            {
                Debug.LogError($"Cannot subtract ResourceAmount values with different resources: '{left.resource}' and '{right.resource}'.");
                return left;
            }

            return new(left.resource, left.amount - right.amount);
        }

        public static implicit operator IntAmount<ResourceSO>(ResourceAmount value) => new(value.resource, value.amount);
        public static implicit operator ResourceAmount(IntAmount<ResourceSO> value) => new(value.item, value.amount);
    }
}
