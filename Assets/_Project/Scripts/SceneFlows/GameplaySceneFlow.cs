using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace ZooTycoon
{
    public class GameplaySceneFlow : SceneFlow
    {
        public override async UniTask PrepareScene(Action<ProgressInfo> progressCallback = null)
        {
            // await UniTask.WaitUntil(() => Services.IsReady);

            // Wait for game systems Start()
            await UniTask.NextFrame();
            await UniTaskUtils.WaitUntilStableFps();
        }

    }
}