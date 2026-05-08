using EditorAttributes;
using UnityEngine;

namespace ZooTycoon
{
    public class FaceCamera : MonoBehaviour
    {
        [SerializeField] private Camera _targetCamera;
        public bool UseUpdate = true;

        private void Start()
        {
            if (_targetCamera == null)
            {
                _targetCamera = Camera.main;
            }

            RotateToCamera();
        }

        private void LateUpdate()
        {
            // LateUpdate is preferred for camera-related movements to ensure the camera has finished its own movement for the frame.
            if (UseUpdate)
            {
                RotateToCamera();
            }
        }

        [Button]
        public void RotateToCamera()
        {
            if (_targetCamera == null) { return; }

            var targetRotation = -_targetCamera.transform.forward;
            targetRotation.y = 0;
            transform.rotation = Quaternion.LookRotation(targetRotation);
        }
    }
}