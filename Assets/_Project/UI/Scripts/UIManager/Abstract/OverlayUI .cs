using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZooTycoon.UI
{
    public interface IOverlayUI
    {
        ShowOverlayBehaviour ShowBehaviour { get; }
    }

    public abstract class OverlayUI : UIView, IOverlayUI
    {
        [SerializeField] private ShowOverlayBehaviour _showBehaviour;
        [field: SerializeField] public bool CanShowMultiple { get; private set; }

        public ShowOverlayBehaviour ShowBehaviour => _showBehaviour;

        public Action OnClose;

        protected override UniTask OnShow() => Anim.PlayAppear();
        protected override UniTask OnHide() => Anim.PlayDisappear();

        public virtual void Click_Close()
        {
            UIManager.HideUI(this);
            OnClose?.Invoke();
            OnClose = null;
        }

    }
}
