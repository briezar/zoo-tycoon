using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameDevKit.ObjectReferences;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ZooTycoon
{
    public class BootstrapSceneFlow : SceneFlow
    {
        [SerializeField] private SceneReference _servicesScene;
        [SerializeField] private SceneReference _nextScene;
        // [SerializeField] private SoundID _bgm;
        // [SerializeField] private PlayerSaveSO _playerSave;

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
            // await UniTask.WaitUntil(() => Services.IsReady);

            // _playerSave.LoadUserData();
            // _playerSave.ImportUserData();

            // UIManager.FadeTransition(FadeSetting.FadeIn(0));

            // var splashUI = UIManager.ShowUI<SplashUI>();

            await UniTaskUtils.WaitUntilStableFps();

            // _bgm.Play();
            // await UIManager.FadeTransition(FadeSetting.FadeOut());

            // splashUI.SetInfo("Loading game systems...");
            // splashUI.RunProgress(0.1f, 1);
            // // await FirebaseService.Init();

            // splashUI.SetInfo("Loading assets...");
            // splashUI.RunProgress(0.9f, 2f);
            var sceneFlow = await LoadScene(_nextScene, LoadSceneMode.Additive);
            sceneFlow.SetActiveScene();

            // await sceneFlow.PrepareScene(info =>
            // {
            //     splashUI.RunProgress(info.TargetProgress, 0.25f);
            //     splashUI.SetInfo(info.Message);
            // });

            // await splashUI.RunProgress(1f, 0.25f);

            // splashUI.SetInfo("Loading complete!");
            // await UniTask.WaitForSeconds(0.2f);

            await sceneFlow.TransitionIn();

            // UIManager.Hide(splashUI);

            await SceneManager.UnloadSceneAsync(gameObject.scene);
            await sceneFlow.TransitionOut();
        }

    }
}