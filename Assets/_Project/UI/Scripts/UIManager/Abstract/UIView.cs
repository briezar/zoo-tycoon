using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevKit.UI;
using GameDevKit;
using UnityEngine.UI;

namespace ZooTycoon.UI
{
    public interface IUIView
    {
        void OnShow();
        void OnHide();
    }

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UIView : AdvancedBehaviour, IUIView
    {
        [field: SerializeField] public bool DestroyOnHide { get; private set; }

        private CanvasGroup _cacheCanvasGroup;
        public CanvasGroup canvasGroup => _cacheCanvasGroup ??= this.GetOrAddComponent<CanvasGroup>();

        private UIAnim _cacheUIanim;
        public UIAnim Anim => _cacheUIanim ??= this.GetOrAddComponent<UIAnim>();

        public RectTransform rectTransform => (RectTransform)transform;

        private Canvas _cacheCanvas;
        public Canvas canvas => _cacheCanvas ??= GetComponent<Canvas>();

        /// <summary> Used to wait for animation and to block user interaction when transitioning </summary>
        public virtual float TransitionInDuration => AnimationTime.DefaultTransitionDuration;

        /// <summary> Used to wait for animation and to block user interaction when transitioning </summary>
        public virtual float TransitionOutDuration => 0;

        /// <summary> Called every ViewManager.Show() </summary>
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        void IUIView.OnShow() => OnShow();
        void IUIView.OnHide() => OnHide();

    }
}