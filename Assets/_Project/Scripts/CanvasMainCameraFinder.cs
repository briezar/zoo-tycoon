using UnityEngine;

namespace ZooTycoon
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasMainCameraFinder : MonoBehaviour
    {
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            _canvas.worldCamera = Camera.main;
        }
    }
}