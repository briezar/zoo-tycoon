using System;
using System.Collections;
using System.Collections.Generic;
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

        protected override void OnShow()
        {
            Anim.PlayAppear();
        }

        protected override void OnHide()
        {
            Anim.PlayDisappear();
        }

        public virtual void Click_Close()
        {
            UIManager.Hide(this);
            OnClose?.Invoke();
            OnClose = null;
        }

    }
}
