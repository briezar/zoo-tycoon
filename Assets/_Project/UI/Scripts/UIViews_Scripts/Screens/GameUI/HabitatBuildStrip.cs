using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameDevKit;
using GameDevKit.Pool;
using PrimeTween;
using UnityEngine;
using ZooTycoon.RuntimeData;

namespace ZooTycoon.UI
{
    /// <summary>
    /// Horizontal habitat-selection strip displayed at the mid-bottom of the GameUI.
    ///
    /// Setup:
    ///   1. Attach this component to the strip's root RectTransform.
    ///   2. Assign _entryPrefab (a prefab with <see cref="HabitatBuildEntry"/>).
    ///   3. Assign _entryContainer (a horizontal layout group child).
    ///   4. Populate _habitatDefinitions with your HabitatDefinitionSO assets.
    ///   5. Optionally assign _playerData; falls back to the global container.
    ///   6. Assign _placementController.
    /// </summary>
    public class HabitatBuildStrip : AdvancedBehaviour
    {
        [SerializeField] private SerializableComponentPool<HabitatBuildEntry> _entryPool;

        [Header("Optional")]
        [SerializeField] private HabitatPlacementController _placementController;
        [SerializeField] private HabitatDefinitionSO[] _habitatDefs;
        [SerializeField] private PlayerRuntimeDataSO _playerData;
        [SerializeField] private GameRuntimeDataSO _gameData;

        public bool IsShowing { get; private set; }

        public RectTransform rectTransform => (RectTransform)transform;

        protected override void OnStart()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerData);
            ScriptableObjectContainer.AssignIfNull(ref _gameData);

            UpdateEntries();
        }

        public async UniTask Show()
        {
            IsShowing = true;
            gameObject.SetActive(true);
            UpdateEntries();
            // await Tween.UIAnchoredPositionY(rectTransform, _initialAnchoredPos.y, 0.5f, Ease.OutSine);
        }

        public async UniTask Hide(bool immediate = false)
        {
            IsShowing = false;
            gameObject.SetActive(false);
            // await Tween.UIAnchoredPositionY(rectTransform, _initialAnchoredPos.y - _height, immediate ? 0 : 0.5f, Ease.InSine);
        }

        public void UpdateEntries()
        {
            IEnumerable<HabitatDefinitionSO> habitatDefs = _gameData?.AvailableHabitats;
            habitatDefs ??= _habitatDefs;
            UpdateEntries(habitatDefs);
        }

        public void UpdateEntries(IEnumerable<HabitatDefinitionSO> habitatDefs)
        {
            _entryPool.ReleaseAll();
            if (habitatDefs.IsNullOrEmpty()) { return; }

            foreach (var definition in habitatDefs)
            {
                var entry = _entryPool.Get();
                entry.Initialize(definition, OnHabitatSelected);
            }
        }

        private void EnsurePlacementController()
        {
            if (_placementController == null)
            {
                _placementController = FindAnyObjectByType<HabitatPlacementController>();
            }
            if (_placementController == null)
            {
                Debug.LogError($"{nameof(HabitatPlacementController)} not found! Please ensure you have one on your scene.");
            }
        }

        private void OnHabitatSelected(HabitatDefinitionSO definition)
        {
            EnsurePlacementController();

            // Guard: only start a new placement if none is active
            if (_placementController.IsPlacing) { return; }

            if (!_playerData.ResourceData.HasEnoughResources(definition.BuildCosts))
            {
                // Button should already be disabled, but guard anyway.
                return;
            }

            _placementController.BeginPlacement(definition);

            Hide();
        }
    }
}
