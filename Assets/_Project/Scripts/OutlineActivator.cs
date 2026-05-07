using EditorAttributes;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZooTycoon
{
    public class OutlineActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private RenderingLayerMask _outlineLayer;

        public Mode ActivationMode = Mode.Hover;

        [ShowField(nameof(ActivationMode), Mode.Click)]
        [Tooltip("If true, clicking will toggle the outline on/off. If false, clicking will only turn the outline on.")]
        public bool ClickToggleOff = true;

        public enum Mode
        {
            Manual,
            Hover,
            Click
        }

        public bool IsOutlineActive { get; private set; }

        private void Reset()
        {
            FetchRenderersInChildren();
        }

        [Button]
        public void FetchRenderersInChildren()
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }

        [Button(serializeParameters: false)]
        public void ToggleOutline(bool isOn)
        {
            IsOutlineActive = isOn;
            foreach (var renderer in _renderers)
            {
                var currentMask = renderer.renderingLayerMask;
                // if active, add outline layer to mask, else remove it
                renderer.renderingLayerMask = isOn ? (currentMask | _outlineLayer) : (uint)(currentMask & ~_outlineLayer);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (ActivationMode is not Mode.Hover) { return; }
            ToggleOutline(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (ActivationMode is not Mode.Hover) { return; }
            ToggleOutline(false);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (ActivationMode is not Mode.Click) { return; }
            if (IsOutlineActive && !ClickToggleOff) { return; }
            ToggleOutline(!IsOutlineActive);
        }
    }
}