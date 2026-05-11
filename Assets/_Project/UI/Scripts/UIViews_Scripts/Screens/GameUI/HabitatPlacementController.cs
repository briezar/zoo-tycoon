using System.Linq;
using Cysharp.Threading.Tasks;
using EditorAttributes;
using GameDevKit;
using UnityEngine;
using UnityEngine.InputSystem;
using ZooTycoon.Input;
using ZooTycoon.QuestSystem;
using ZooTycoon.RuntimeData;

namespace ZooTycoon.UI
{
    /// <summary>
    /// Drives the full habitat placement flow:
    ///
    ///   1. <see cref="BeginPlacement"/> spawns a preview prefab that follows the mouse.
    ///   2. First left-click while in FOLLOWING state → show confirm popup above preview.
    ///   3. Accept  → spawn real habitat, deduct resources, end placement.
    ///      Cancel  → destroy preview, end placement.
    ///
    /// Setup:
    ///   - Attach to any persistent GameObject in the GameUI scene.
    ///   - Assign _confirmPopup (the <see cref="HabitatPlacementConfirmPopup"/> prefab/instance).
    ///   - Assign _playerData (or leave null for global-container lookup).
    ///   - Assign _placementCamera (the camera used to translate mouse → world position).
    ///   - Set _groundLayerMask to the layer your terrain/ground sits on.
    ///   - Set _popupWorldOffset to shift the popup above the preview (e.g. 0, 3, 0).
    /// </summary>
    public class HabitatPlacementController : AdvancedBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────────────────
        [SerializeField] private Transform _canvas;
        [SerializeField] private HabitatPlacementConfirmPopup _confirmPopup;
        [SerializeField] private Grid _grid;

        [SerializeField] private QuestObjectiveDefinitionSO _buildHabitatObjective;

        [TagDropdown]
        [SerializeField] private string _placementCameraTag = "MainCamera";
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private Vector3 _popupWorldOffset = new(0f, 3f, 0f);

        [Header("Optional")]
        [SerializeField] private PlayerRuntimeDataSO _playerData;

        public bool IsPlacing => _state != PlacementState.Idle;

        private enum PlacementState { Idle, Following, AwaitingConfirm }

        private PlacementState _state = PlacementState.Idle;
        private HabitatDefinitionSO _currentDefinition;
        private GameObject _previewInstance;
        private Camera _placementCamera;


        // ── Lifecycle ────────────────────────────────────────────────────────────
        protected override void OnStart()
        {
            ScriptableObjectContainer.AssignIfNull(ref _playerData);

            if (_placementCamera == null)
            {
                _placementCamera = Camera.allCameras.Find(c => c.CompareTag(_placementCameraTag));
            }

            // Popup starts hidden
            _confirmPopup.gameObject.SetActive(false);
        }

        private void Update()
        {
            switch (_state)
            {
                case PlacementState.Following:
                    MovePreviewToMouse();
                    HandleFollowingClick();
                    break;

                case PlacementState.AwaitingConfirm:
                    // Keep popup positioned above preview while waiting
                    PositionPopupAbovePreview();
                    break;
            }
        }

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>Called by <see cref="HabitatBuildStrip"/> when the player selects a habitat card.</summary>
        public void BeginPlacement(HabitatDefinitionSO definition)
        {
            if (IsPlacing) { return; }

            InputManager.Enable_PlayerMovement(false);

            _currentDefinition = definition;

            _previewInstance = Instantiate(definition.PreviewPrefab, GetPlacementPosition(), Quaternion.identity);

            _state = PlacementState.Following;
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private void MovePreviewToMouse()
        {
            if (_previewInstance == null) { return; }
            _previewInstance.transform.position = GetPlacementPosition();
        }

        private void HandleFollowingClick()
        {
            // Ignore if pointer is over a UI element so the strip itself doesn't trigger placement
            if (InputManager.IsPointerOverUI()) { return; }

            if (Pointer.current.press.wasPressedThisFrame)
            {
                ShowConfirmPopupAsync();
            }
        }

        private async UniTaskVoid ShowConfirmPopupAsync()
        {
            _state = PlacementState.AwaitingConfirm;

            PositionPopupAbovePreview();

            await _confirmPopup.ShowAsync(
                onAccept: () => ConfirmPlacement(),
                onCancel: () => CancelPlacement()
            );
        }

        private void ConfirmPlacement()
        {
            HidePopupAndCleanupAsync(confirmed: true).Forget();
        }

        private void CancelPlacement()
        {
            HidePopupAndCleanupAsync(confirmed: false).Forget();
        }

        private async UniTaskVoid HidePopupAndCleanupAsync(bool confirmed)
        {
            await _confirmPopup.HideAsync();
            InputManager.Enable_PlayerMovement(true);

            if (confirmed)
            {
                SpawnHabitat();
                DeductResources();
            }

            DestroyPreview();
            _state = PlacementState.Idle;
            _currentDefinition = null;
        }

        private void SpawnHabitat()
        {
            if (_previewInstance == null || _currentDefinition == null) { return; }
            Instantiate(_currentDefinition.HabitatPrefab, _previewInstance.transform.position, _previewInstance.transform.rotation);

            QuestManager.Instance.IncreaseObjective(_buildHabitatObjective, 1);
        }

        private void DeductResources()
        {
            if (_currentDefinition == null) { return; }

            _playerData.ResourceData.AddResources(_currentDefinition.BuildCosts.Select(c => c.Invert()));
        }

        private void DestroyPreview()
        {
            if (_previewInstance != null)
            {
                Destroy(_previewInstance);
                _previewInstance = null;
            }
        }

        private void PositionPopupAbovePreview()
        {
            if (_previewInstance == null || _confirmPopup == null) { return; }
            _canvas.position = _previewInstance.transform.position + _popupWorldOffset;
        }

        private Vector3 GetPlacementPosition()
        {
            var ray = _placementCamera.ScreenPointToRay(Pointer.current.position.ReadValue());

            Vector3 targetPos;
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _groundLayerMask))
            {
                targetPos = hit.point;
            }
            else
            {
                // Fallback: project onto y=0 plane
                var t = -ray.origin.y / ray.direction.y;
                targetPos = ray.origin + ray.direction * t;
            }

            if (_grid != null)
            {
                targetPos = _grid.WorldToCellCenterWorld(targetPos);
            }

            return targetPos;
        }

    }
}
