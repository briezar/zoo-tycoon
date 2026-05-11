using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameDevKit.ObjectReferences;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZooTycoon.UI;

namespace ZooTycoon
{
    public class BootstrapSceneFlow : SceneFlow
    {
        [SerializeField] private SceneReference _servicesScene;
        [SerializeField] private SceneReference _nextScene;

#if UNITY_EDITOR
        private void Awake()
        {
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.LoadScene(gameObject.scene.name);
                return;
            }
        }
#endif

        private async UniTaskVoid Start()
        {
            SceneManager.LoadScene(_servicesScene, LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => UIManager.IsReady);

            UIManager.FadeTransition(FadeSetting.FadeIn(0));

            var splashUI = UIManager.ShowUI<SplashUI>();

            await UniTask.NextFrame();
            await UniTaskUtils.WaitUntilStableFps();

            await UIManager.FadeTransition(FadeSetting.FadeOut());

            await UniTask.WaitForSeconds(0.2f);
            splashUI.SetInfo("Loading game systems...");
            splashUI.RunProgress(0.2f, 1);

            var nextSceneFlow = await LoadScene(_nextScene, LoadSceneMode.Additive);
            nextSceneFlow.SetActiveScene();

            await nextSceneFlow.PrepareScene(info =>
            {
                splashUI.RunProgress(info.TargetProgress, 0.25f);
                splashUI.SetInfo(info.Message);
            });

            await splashUI.RunProgress(1f, 0.25f);

            splashUI.SetInfo("Loading complete!");
            await UniTask.WaitForSeconds(0.2f);

            await nextSceneFlow.TransitionIn();

            UIManager.HideUI(splashUI);

            await SceneManager.UnloadSceneAsync(gameObject.scene);
            await nextSceneFlow.TransitionOut();
        }

    }
}