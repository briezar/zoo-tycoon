using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace ZooTycoon
{
    public class ProgressBar : MonoBehaviour
    {
        [field: SerializeField] public Slider Slider { get; private set; }

        /// <summary>
        /// From 0f-1f.
        /// </summary>
        public float Progress
        {
            get => Slider.normalizedValue;
            set
            {
                StopProgress();
                Slider.normalizedValue = value;
            }
        }

        private Tween _progressTween;

        public void StopProgress()
        {
            if (_progressTween.isAlive)
            {
                _progressTween.Stop();
            }
        }

        public UniTask RunProgressNormalized(float to, float duration) => RunProgressNormalized(Slider.normalizedValue, to, duration);
        public async UniTask RunProgressNormalized(float from, float to, float duration)
        {
            _progressTween = Tween.Custom(Slider, from, to, duration, (slider, value) => slider.normalizedValue = value);
            await _progressTween;
        }
    }
}