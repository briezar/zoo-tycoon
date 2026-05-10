using System;
using EditorAttributes;
using GameDevKit;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace ZooTycoon
{
    [CreateAssetMenu(menuName = $"{ScriptableObjectConstants.MenuName}/Resource")]
    public class ResourceSO : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }

        [field: AssetPreview(24, 24)]
        [field: SerializeField] public Sprite Icon { get; private set; }

        [SerializeField] private TMP_SpriteAsset _spriteAsset;
        [SerializeField] private string _spriteName;
        [SerializeField] private Color _textColor;

        [Button]
        private void DebugIconText(int amount = 123)
        {
            var index = _spriteAsset.GetSpriteIndexFromName(_spriteName);
            if (index < 0)
            {
                Debug.LogError($"{_spriteName} does not exist in {_spriteAsset}", _spriteAsset);
                return;
            }

            Debug.Log(GetIconAmountText(amount, true));
        }

        public string GetIconText() => RichTextUtils.EvaluateSpriteAsset(_spriteAsset.name, _spriteName);
        public string GetAmountText(int amount) => $"{amount.ToString().Colorize(_textColor)}";

        /// <summary>
        /// E.g. 🪙15
        /// </summary>
        /// <param name="prefixIcon">Adds icon to the front or back</param>
        public string GetIconAmountText(int amount, bool prefixIcon = true)
        {
            var iconText = GetIconText();
            var amountText = GetAmountText(amount);
            return prefixIcon ? iconText + amountText : amountText + iconText;
        }
    }

    [Serializable]
    public struct ResourceAmount
    {
        public ResourceSO resource;
        public int amount;

        public readonly ResourceAmount With(int newAmount) => new(resource, newAmount);
        public readonly ResourceAmount Invert() => new(resource, -amount);

        public ResourceAmount(ResourceSO resource, int amount = 0) => (this.resource, this.amount) = (resource, amount);

        /// <inheritdoc cref="ResourceSO.GetIconAmountText(int, bool)" />
        public readonly string GetIconAmountText(bool prefixIcon = true) => resource.GetIconAmountText(amount, prefixIcon);

        private static void AssertEqualResource(ResourceAmount left, ResourceAmount right)
        {
            Assert.AreEqual(left.resource, right.resource, $"Cannot operate on ResourceAmount values with different resources: '{left.resource}' != '{right.resource}'");
        }

        public static ResourceAmount operator +(ResourceAmount left, ResourceAmount right) => left.AsIntAmount() + right.AsIntAmount();
        public static ResourceAmount operator -(ResourceAmount left, ResourceAmount right) => left.AsIntAmount() + right.AsIntAmount();
        public static bool operator >(ResourceAmount left, ResourceAmount right) => left.AsIntAmount() > right.AsIntAmount();
        public static bool operator <(ResourceAmount left, ResourceAmount right) => left.AsIntAmount() < right.AsIntAmount();
        public static bool operator >=(ResourceAmount left, ResourceAmount right) => left.AsIntAmount() >= right.AsIntAmount();
        public static bool operator <=(ResourceAmount left, ResourceAmount right) => left.AsIntAmount() <= right.AsIntAmount();

        private readonly IntAmount<ResourceSO> AsIntAmount() => (IntAmount<ResourceSO>)this;

        public static implicit operator IntAmount<ResourceSO>(ResourceAmount value) => new(value.resource, value.amount);
        public static implicit operator ResourceAmount(IntAmount<ResourceSO> value) => new(value.item, value.amount);
    }
}
