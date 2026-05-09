using System;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using GameDevKit;
using UnityEngine;

namespace ZooTycoon.RuntimeData
{
    [Serializable]
    public class PlayerResourceData
    {
        [SerializeField] private List<ResourceAmount> _currentAmounts;
        [SerializeField] private List<ResourceAmount> _maxAmounts;

        public IReadOnlyList<ResourceAmount> CurrentAmounts => _currentAmounts;
        public IReadOnlyList<ResourceAmount> MaxAmounts => _maxAmounts;

        public readonly SourcedAction<ResourceSO, IntChangeInfo> OnCurrentAmountChanged = new();
        public readonly SourcedAction<ResourceSO, IntChangeInfo> OnMaxAmountChanged = new();

        public void SetMaxResourceAmount(ResourceAmount maxResourceAmount)
        {
            var resource = maxResourceAmount.resource;
            var index = _maxAmounts.FindIndex(r => r.resource == maxResourceAmount.resource);
            var prevAmount = _maxAmounts[index];
            if (prevAmount.amount == maxResourceAmount.amount) { return; }

            _maxAmounts[index] = maxResourceAmount;
            OnMaxAmountChanged?.Invoke(resource, new(prevAmount.amount, maxResourceAmount.amount));
        }

        public void SetResource(ResourceAmount newResourceAmount) => SetResource_Internal(newResourceAmount);
        public void AddResource(ResourceAmount resourceAmountToAdd) => SetResource_Internal(_currentAmounts.Get(resourceAmountToAdd.resource) + resourceAmountToAdd);

        public void AddResources(params ResourceAmount[] rewards) => AddResources(rewards.AsEnumerable());
        public void AddResources(IEnumerable<ResourceAmount> rewards)
        {
            foreach (var reward in rewards)
            {
                AddResource(reward);
            }
        }

        private void SetResource_Internal(ResourceAmount newResourceAmount, bool notify = true)
        {
            var resource = newResourceAmount.resource;
            var resourceAmountIndex = _currentAmounts.FindIndex(r => r.resource == resource);
            var maxResourceAmountIndex = _maxAmounts.FindIndex(r => r.resource == resource);
            if (resourceAmountIndex < 0 || maxResourceAmountIndex < 0)
            {
                Debug.LogWarning($"Missing config for {resource}!");
                return;
            }

            var prevAmount = _currentAmounts[resourceAmountIndex];
            var maxAmount = _maxAmounts[maxResourceAmountIndex];

            var clampedResourceAmount = new ResourceAmount(resource, newResourceAmount.amount.ClampMax(maxAmount.amount));
            _currentAmounts[resourceAmountIndex] = clampedResourceAmount;

            if (notify)
            {
                OnCurrentAmountChanged?.Invoke(resource, new(prevAmount.amount, clampedResourceAmount.amount));
            }
        }

    }

    public static class PlayerResourceDataExtensions
    {
        /// <summary> If not found, returns <see cref="ResourceAmount.amount"/>==0 since <see cref="ResourceAmount"/> is a struct </summary>
        public static ResourceAmount Get(this IReadOnlyList<ResourceAmount> resourceAmounts, ResourceSO resource) => resourceAmounts.FirstOrDefault(r => r.resource == resource);
    }
}