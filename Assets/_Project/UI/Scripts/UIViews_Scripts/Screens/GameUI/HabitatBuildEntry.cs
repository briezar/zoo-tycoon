using System;
using GameDevKit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZooTycoon.RuntimeData;

namespace ZooTycoon.UI
{
    /// <summary>
    /// Represents a single habitat card in the HabitatBuildStrip.
    /// Wired up automatically by HabitatBuildStrip; no manual setup needed.
    /// </summary>
    public class HabitatBuildEntry : AdvancedBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _affordableState;   // shown when player can afford
        [SerializeField] private GameObject _unaffordableState; // shown when player cannot afford

        [Header("Optional")]
        [SerializeField] private HabitatDefinitionSO _definition;
        [SerializeField] private PlayerRuntimeDataSO _playerData;

        protected override void OnStart()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerData);
        }

        protected override void OnStartOrEnable()
        {
            if (_playerData == null) { return; }
            _playerData.ResourceData.OnCurrentAmountChanged[this] += (_, _) => RefreshAffordability();
        }

        private void OnDisable()
        {
            _playerData?.ResourceData.OnCurrentAmountChanged.UnsubscribeSource(this);
        }

        /// <summary>Called by <see cref="HabitatBuildStrip"/> after instantiation.</summary>
        public void Initialize(HabitatDefinitionSO definition, Action<HabitatDefinitionSO> onSelected)
        {
            _definition = definition;

            _icon.sprite = definition.Icon;
            _nameText.text = definition.DisplayName;
            _costText.text = GetCostString(definition);

            _button.onClick.AddListener(() => onSelected?.Invoke(_definition));

            ScriptableObjectContainer.AssignIfNull(ref _playerData);
            RefreshAffordability();
        }

        private void RefreshAffordability()
        {
            if (_playerData == null || _definition == null) { return; }

            bool canAfford = _playerData.ResourceData.HasEnoughResources(_definition.BuildCosts);
            _affordableState?.SetActive(canAfford);
            _unaffordableState?.SetActive(!canAfford);
            _button.interactable = canAfford;
        }

        private static string GetCostString(HabitatDefinitionSO def)
        {
            using var _ = StringBuilderPool.Get(out var stringBuilder);
            foreach (var cost in def.BuildCosts)
            {
                if (stringBuilder.Length > 0) { stringBuilder.Append('\n'); }
                stringBuilder.Append($"{cost.GetIconAmountText()}");
            }
            return stringBuilder.ToString();
        }
    }
}
