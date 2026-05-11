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
        [SerializeField] private CanvasGroup _bgDim;
        [SerializeField] private CanvasGroup _screenFader;

        private OverlayBgDim? _popupBgDimCache;
        public OverlayBgDim PopupBgDim
        {
            get
            {
                _popupBgDimCache ??= new OverlayBgDim
                {
                    gameObject = _bgDim.gameObject,
                    canvas = _bgDim.GetComponent<Canvas>()
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

        public Tween FadeOverlayUIDim(bool fadeIn, float duration = AnimationTime.DefaultTransitionDuration)
        {
            _bgDim.gameObject.SetActive(true);
            var tween = Tween.Alpha(_bgDim, fadeIn ? 1 : 0, duration).OnComplete(() => _bgDim.gameObject.SetActive(fadeIn));
            return tween;
        }

    }
}