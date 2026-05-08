using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameDevKit.ObjectReferences;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZooTycoon
{
    public record struct ProgressInfo(float TargetProgress, float EstimatedDuration, string Message);

    [DefaultExecutionOrder(-1000)]
    public abstract class SceneFlow : MonoBehaviour
    {
        protected static readonly List<SceneFlow> _activeSceneFlows = new();

        private void OnEnable() => _activeSceneFlows.Add(this);
        private void OnDisable() => _activeSceneFlows.Remove(this);

        public virtual UniTask PrepareScene(Action<ProgressInfo> progressCallback = null) => UniTask.CompletedTask;
        public virtual UniTask TransitionIn()
        {
            return UniTask.CompletedTask;
            // return UIManager.FadeTransition(FadeSetting.FadeIn());
        }

        public virtual UniTask TransitionOut()
        {
            return UniTask.CompletedTask;
            // return UIManager.FadeTransition(FadeSetting.FadeOut());
        }

        public void SetActiveScene()
        {
            SceneManager.SetActiveScene(gameObject.scene);
        }

        public static async UniTask<SceneFlow> LoadScene(SceneReference sceneRef, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            await SceneManager.LoadSceneAsync(sceneRef, mode);
            var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            return FindInScene(scene);
        }

        public static SceneFlow FindInScene(Scene scene)
        {
            foreach (var obj in scene.GetRootGameObjects())
            {
                if (obj.TryGetComponentInChildren(out SceneFlow sceneFlow))
                {
                    return sceneFlow;
                }
            }
            return null;
        }
    }
}