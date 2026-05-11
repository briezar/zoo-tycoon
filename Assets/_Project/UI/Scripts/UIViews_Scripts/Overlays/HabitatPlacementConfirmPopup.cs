using System;
using Cysharp.Threading.Tasks;
using GameDevKit;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace ZooTycoon.UI
{
    /// <summary>
    /// Small world-space (or screen-space overlay) popup shown above a habitat preview
    /// asking the player to confirm or cancel placement.
    ///
    /// Setup:
    ///   - Place as a child of a Screen-Space Overlay canvas OR as a world-space canvas
    ///     parented to the preview prefab.
    ///   - Assign _acceptButton and _cancelButton.
    ///   - The popup scales in/out via PrimeTween for polish.
    /// </summary>
    public class HabitatPlacementConfirmPopup : AdvancedBehaviour
    {
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private RectTransform _panel;

        private Action _onAccept;
        private Action _onCancel;

        protected override void OnStart()
        {
            _acceptButton.onClick.AddListener(HandleAccept);
            _cancelButton.onClick.AddListener(HandleCancel);
            _panel.localScale = Vector3.zero;
        }

        /// <summary>Shows the popup with a bounce-in animation.</summary>
        public async UniTask ShowAsync(Action onAccept, Action onCancel)
        {
            _onAccept = onAccept;
            _onCancel = onCancel;

            gameObject.SetActive(true);
            await Tween.Scale(_panel, Vector3.zero, Vector3.one, 0.3f, Ease.OutBack);
        }

        /// <summary>Hides the popup with a shrink-out animation then deactivates.</summary>
        public async UniTask HideAsync()
        {
            await Tween.Scale(_panel, Vector3.one, Vector3.zero, 0.2f, Ease.InBack);
            gameObject.SetActive(false);
        }

        private void HandleAccept()
        {
            var cb = _onAccept;
            _onAccept = null;
            _onCancel = null;
            cb?.Invoke();
        }

        private void HandleCancel()
        {
            var cb = _onCancel;
            _onAccept = null;
            _onCancel = null;
            cb?.Invoke();
        }
    }
}
