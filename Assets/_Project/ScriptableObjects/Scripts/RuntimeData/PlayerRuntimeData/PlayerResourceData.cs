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
        [SerializeField] private List<ResourceAmount> _resourceAmounts;

        public IReadOnlyList<ResourceAmount> ResourceAmounts => _resourceAmounts;

        public readonly SourcedAction<ResourceSO, IntChangeInfo> OnResourceChanged = new();

        public ResourceAmount GetResourceAmount(ResourceSO resource) => _resourceAmounts[GetAndEnsureResourceIndex(resource)];
        public int GetResourceQuantity(ResourceSO resource) => GetResourceAmount(resource).amount;

        public void SetResource(ResourceAmount newResourceAmount) => SetResource_Internal(GetAndEnsureResourceIndex(newResourceAmount), newResourceAmount);

        public void AddResource(ResourceAmount resourceAmountToAdd)
        {
            var resourceIndex = GetAndEnsureResourceIndex(resourceAmountToAdd);
            SetResource_Internal(resourceIndex, _resourceAmounts[resourceIndex] + resourceAmountToAdd);
        }

        public void AddResources(params ResourceAmount[] rewards) => AddResources(rewards.AsEnumerable());
        public void AddResources(IEnumerable<ResourceAmount> rewards)
        {
            foreach (var reward in rewards)
            {
                AddResource(reward);
            }
        }

        private void SetResource_Internal(int resourceIndex, ResourceAmount newResourceAmount, bool notify = true)
        {
            var prevResourceAmount = _resourceAmounts[resourceIndex].amount;
            _resourceAmounts[resourceIndex] = newResourceAmount;

            if (notify)
            {
                OnResourceChanged?.Invoke(newResourceAmount.resource, new(prevResourceAmount, newResourceAmount.amount));
            }
        }

        private int GetAndEnsureResourceIndex(ResourceAmount resourceAmount) => GetAndEnsureResourceIndex(resourceAmount.resource);
        private int GetAndEnsureResourceIndex(ResourceSO resource)
        {
            var index = _resourceAmounts.FindIndex(r => r.resource == resource);
            if (index < 0)
            {
                var resourceAmount = new ResourceAmount(resource);
                _resourceAmounts.Add(resourceAmount);
                index = _resourceAmounts.Count - 1;
            }
            return index;
        }

    }
}