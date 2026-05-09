using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using GameDevKit;
using Cysharp.Threading.Tasks;

namespace ZooTycoon.UI
{
    [Serializable]
    public class UIManagerAnimation
    {
        [SerializeField] private CanvasGroup _popupBgDim;
        [SerializeField] private CanvasGroup _screenFader;

        private PopupBgDim? _popupBgDimCache;
        public PopupBgDim PopupBgDim
        {
            get
            {
                _popupBgDimCache ??= new PopupBgDim
                {
                    gameObject = _popupBgDim.gameObject,
                    canvas = _popupBgDim.GetComponent<Canvas>()
                };
                return _popupBgDimCache.Value;
            }
        }

        public async UniTask FadeTransition(FadeSetting fadeSetting)
        {
            if (fadeSetting.FadeInDuration != null)
            {
                await FadeScreen(true, fadeSetting.FadeInDuration.Value);
                fadeSetting.OnFadeInComplete?.Invoke();
            }

            await UniTask.WaitForSeconds(fadeSetting.WaitAfterFadeIn);

            if (fadeSetting.FadeOutCondition != null)
            {
                await UniTask.WaitUntil(fadeSetting.FadeOutCondition);
            }

            if (fadeSetting.FadeOutDuration != null)
            {
                fadeSetting.OnFadeOutStart?.Invoke();
                await FadeScreen(false, fadeSetting.FadeOutDuration.Value);
            }

            fadeSetting.OnFinish?.Invoke();
        }

        public Tween FadeScreen(bool fadeIn, float duration = 0)
        {
            _screenFader.gameObject.SetActive(true);
            var tween = Tween.Alpha(_screenFader, fadeIn ? 1 : 0, duration).OnComplete(() => _screenFader.gameObject.SetActive(fadeIn));
            return tween;
        }

        public Tween FadePopupDim(bool fadeIn, float duration = AnimationTime.DefaultTransitionDuration)
        {
            _popupBgDim.gameObject.SetActive(true);
            var tween = Tween.Alpha(_popupBgDim, fadeIn ? 1 : 0, duration).OnComplete(() => _popupBgDim.gameObject.SetActive(fadeIn));
            return tween;
        }

    }
}