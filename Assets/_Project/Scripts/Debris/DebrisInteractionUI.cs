using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameDevKit.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ZooTycoon
{
    public class DebrisInteractionUI : MonoBehaviour, IModalInteractionUI
    {
        [field: SerializeField] public TextButton ClearDebrisBtn { get; private set; }

        private void OnEnable() => IInteractionUI.ActiveUIs.Add(this);
        private void OnDisable() => IInteractionUI.ActiveUIs.Remove(this);

        private Canvas _canvas;
        private Transform _canvasParent;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasParent = _canvas.transform.parent;
        }

        public async UniTask Show()
        {
            gameObject.SetActive(true);
            _canvas.transform.SetParentKeepPosition(null);
        }

        public async UniTask Hide()
        {
            gameObject.SetActive(false);
            _canvas.transform.SetParentKeepPosition(_canvasParent);
        }
    }
}