using System.Collections.Generic;
using EditorAttributes;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZooTycoon
{
    public class OutlineActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public enum Mode { Manual, Hover, Click }

        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private RenderingLayerMask _outlineLayer;

        public Mode ActivationMode = Mode.Hover;

        [ShowField(nameof(ActivationMode), Mode.Click)]
        [Tooltip("If true, clicking will toggle the outline on/off. If false, clicking will only turn the outline on.")]
        public bool ClickToggleOff = true;

        public bool IsOutlineActive => _usageCounter.IsUsing;

        private readonly UsageCounter _usageCounter = new();

        private static readonly HashSet<OutlineActivator> _instances = new();

        private void Reset()
        {
            FetchRenderersInChildren();
        }

        private void Awake() => _instances.Add(this);
        private void OnDestroy() => _instances.Remove(this);

        [Button]
        public void FetchRenderersInChildren()
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }

        public void ToggleOutline() => ActivateOutline(!IsOutlineActive);

        [Button(serializeParameters: false)]
        public void ActivateOutline(bool active)
        {
            var isUsing = _usageCounter.Use(active, out var usingStateChanged);
            if (!usingStateChanged) { return; }

            foreach (var renderer in _renderers)
            {
                var currentMask = renderer.renderingLayerMask;
                // if active, add outline layer to mask, else remove it
                renderer.renderingLayerMask = isUsing ? (currentMask | _outlineLayer) : (uint)(currentMask & ~_outlineLayer);
            }
        }

        [Button]
        public void ActivateOutlineSolo()
        {
            foreach (var instance in _instances)
            {
                instance.ActivateOutline(instance == this);
            }
        }

        public static void DeactivateAllOutlines()
        {
            foreach (var instance in _instances)
            {
                instance.ActivateOutline(false);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (ActivationMode is not Mode.Hover) { return; }
            ActivateOutline(true);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (ActivationMode is not Mode.Hover) { return; }
            ActivateOutline(false);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (ActivationMode is not Mode.Click) { return; }
            if (IsOutlineActive && !ClickToggleOff) { return; }
            ActivateOutline(!IsOutlineActive);
        }
    }
}