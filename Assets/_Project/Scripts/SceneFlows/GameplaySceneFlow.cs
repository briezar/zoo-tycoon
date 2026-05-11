using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using ZooTycoon.UI;

namespace ZooTycoon
{
    public class GameplaySceneFlow : SceneFlow
    {
        private void Start()
        {
            PrimeTweenConfig.warnTweenOnDisabledTarget = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            PrepareScene();
        }

        public override async UniTask PrepareScene(Action<ProgressInfo> progressCallback = null)
        {
            // await UniTask.WaitUntil(() => Services.IsReady);

            UIManager.ShowUI<GameUI>();

            // Wait for game systems Start()
            await UniTask.NextFrame();
            await UniTaskUtils.WaitUntilStableFps();
        }

    }
}