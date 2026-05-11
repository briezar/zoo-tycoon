using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZooTycoon.UI
{
    public interface IPopupUI
    {
        ShowPopupBehaviour ShowPopupBehaviour { get; }
    }

    public abstract class PopupUI : UIView, IPopupUI
    {
        [SerializeField] private ShowPopupBehaviour _showPopupBehaviour;
        [field: SerializeField] public bool CanShowMultiple { get; private set; }

        public ShowPopupBehaviour ShowPopupBehaviour => _showPopupBehaviour;

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
