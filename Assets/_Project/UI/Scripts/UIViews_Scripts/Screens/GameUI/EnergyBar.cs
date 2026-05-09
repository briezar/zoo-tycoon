using System;
using GameDevKit;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ZooTycoon.RuntimeData;

namespace ZooTycoon.UI
{
    public class EnergyBar : AdvancedBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private PlayerRuntimeDataSO _playerRuntimeData;
        [SerializeField] private Slider _energySlider;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private Image _fillImg;

        [Tooltip("What color the bar would look like when below a certain amount (1 to 0). Must be descending.")]
        [SerializeField] private FloatAmount<Color>[] _progressColors;

        protected override void OnStart()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerRuntimeData);
            _energyText.gameObject.SetActive(false);
            _energySlider.onValueChanged.AddListener((_) =>
            {
                UpdateFillColor();
                UpdateEnergyText();
            });
        }

        protected override void OnStartOrEnable()
        {
            _playerRuntimeData.ResourceData.OnCurrentAmountChanged[this] += HandleResourceChanged;
            _playerRuntimeData.ResourceData.OnMaxAmountChanged[this] += HandleMaxResourceChanged;

            _energySlider.minValue = 0;
            _energySlider.maxValue = _playerRuntimeData.ResourceData.MaxAmounts.Get(ResourceSO_Ref.Energy).amount;
            _energySlider.value = _playerRuntimeData.ResourceData.CurrentAmounts.Get(ResourceSO_Ref.Energy).amount;
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _playerRuntimeData.ResourceData.AddResource(new(ResourceSO_Ref.Energy, -10));
            }
        }

        private void OnDisable()
        {
            _playerRuntimeData.ResourceData.OnCurrentAmountChanged.Unsubscribe(this);
        }

        private void HandleResourceChanged(ResourceSO resource, IntChangeInfo info)
        {
            if (resource != ResourceSO_Ref.Energy) { return; }
            Tween.UISliderValue(_energySlider, info.previous, info.current, 0.5f);
        }

        private void HandleMaxResourceChanged(ResourceSO resource, IntChangeInfo info) => _energySlider.maxValue = info.current;

        private Color GetProgressColor(float ratio)
        {
            for (int i = _progressColors.Length - 1; i >= 0; i--)
            {
                var progressColor = _progressColors[i];
                if (ratio <= progressColor.amount)
                {
                    return progressColor.item;
                }
            }
            return _progressColors[0].item;
        }

        private void UpdateFillColor() => _fillImg.color = GetProgressColor(_energySlider.normalizedValue);
        private void UpdateEnergyText() => _energyText.text = $"{_energySlider.value}";

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _energyText.gameObject.SetActive(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _energyText.gameObject.SetActive(false);
        }
    }
}