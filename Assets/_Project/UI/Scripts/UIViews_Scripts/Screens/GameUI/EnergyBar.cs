using System;
using GameDevKit;
using UnityEngine;
using UnityEngine.UI;
using ZooTycoon.RuntimeData;

namespace ZooTycoon.UI
{
    public class EnergyBar : AdvancedBehaviour
    {
        [SerializeField] private PlayerRuntimeDataSO _playerRuntimeData;
        [SerializeField] private Slider _energySlider;

        [Tooltip("What color the bar would look like when below a certain amount (1 to 0). Must be descending.")]
        [SerializeField] private FloatAmount<Color>[] _progressColors;

        protected override void OnStart()
        {
            _playerRuntimeData ??= RuntimeDataContainer.FindData<PlayerRuntimeDataSO>();
        }

        protected override void OnStartOrEnable()
        {
            _playerRuntimeData.Resource.OnResourceChanged[this] += HandleResourceChanged;
        }

        private void OnDisable()
        {
            _playerRuntimeData.Resource.OnResourceChanged.Unsubscribe(this);
        }

        private void HandleResourceChanged(ResourceSO resource, IntChangeInfo info)
        {
            
        }
    }
}