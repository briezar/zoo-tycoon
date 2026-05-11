using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using ZooTycoon.QuestSystem;
using ZooTycoon.UI;

namespace ZooTycoon
{
    public class GameplaySceneFlow : SceneFlow
    {
        [SerializeField] private CinemachineCamera _cineCam;
        [SerializeField] private TweenSettings<float> _cameraOrthoTweenSettings;
        [SerializeField] private StoryDirector _storyDirector;

        private async UniTaskVoid Start()
        {
            PrimeTweenConfig.warnTweenOnDisabledTarget = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            _cineCam.Lens.OrthographicSize = _cameraOrthoTweenSettings.startValue;

            if (_activeSceneFlows.Count == 1)
            {
                await PrepareScene();
                await TransitionOut();
            }
        }

        public override async UniTask TransitionOut()
        {
            UIManager.ShowUI<GameUI>();
            base.TransitionOut();
            await Tween.Custom(_cameraOrthoTweenSettings, (value) => _cineCam.Lens.OrthographicSize = value);
            _storyDirector.gameObject.SetActive(true);
        }

        public override async UniTask PrepareScene(Action<ProgressInfo> progressCallback = null)
        {
            progressCallback?.Invoke(new(0.8f, 0.5f, "Loading UI..."));
            UIManager.PreloadUI<GameUI>();

            // Wait for game systems Start()
            await UniTask.NextFrame();
            await UniTaskUtils.WaitUntilStableFps();

            await UniTask.WaitForSeconds(0.25f);
            progressCallback?.Invoke(new(1f, 0.2f, "Loading UI..."));
        }

    }
}