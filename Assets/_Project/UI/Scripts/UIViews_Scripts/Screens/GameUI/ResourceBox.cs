using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameDevKit;
using PrimeTween;
using TMPro;
using UnityEngine;
using ZooTycoon.RuntimeData;

namespace ZooTycoon.UI
{
    public class ResourceBox : AdvancedBehaviour
    {
        [SerializeField] private ResourceSO _resource;
        [SerializeField] private TMP_Text _amountText;

        [Header("Optional")]
        [SerializeField] private PlayerRuntimeDataSO _playerData;

        private Tween _runTextTween;

        protected override void OnStart()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerData);
        }

        protected override void OnStartOrEnable()
        {
            _playerData.ResourceData.OnCurrentAmountChanged[this] += HandleResourceChanged;
            UpdateResourceText(_playerData.ResourceData.CurrentAmounts.Get(_resource).amount);
        }

        private void OnDisable()
        {
            _playerData.ResourceData.OnCurrentAmountChanged.Clear(this);
        }


        private void HandleResourceChanged(ResourceSO resource, IntChangeInfo info)
        {
            if (resource != _resource) { return; }
            _runTextTween.Stop();
            _runTextTween = Tween.Custom(_amountText.text.ToInt(), info.current, 1f, (value) => UpdateResourceText(Mathf.FloorToInt(value)), Ease.OutExpo);
            Tween.Scale(_amountText.transform, Vector3.one * 1.3f, 0.3f, Ease.OutSine, 2, CycleMode.Rewind);
        }

        private void UpdateResourceText(int amount)
        {
            _amountText.text = $"{amount}";
        }

    }
}