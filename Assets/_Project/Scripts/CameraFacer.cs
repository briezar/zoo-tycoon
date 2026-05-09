using EditorAttributes;
using UnityEngine;

namespace ZooTycoon
{
    public class CameraFacer : MonoBehaviour
    {
        [HelpBox("Defaults to Camera.main if not assigned")]
        public Camera TargetCamera;
        public bool FaceOnStart = true;
        public bool UseUpdate = true;

        private void Start()
        {
            if (TargetCamera == null)
            {
                TargetCamera = Camera.main;
            }

            if (FaceOnStart) { FaceCamera(); }
        }

        private void LateUpdate()
        {
            // LateUpdate is preferred for camera-related movements to ensure the camera has finished its own movement for the frame.
            if (UseUpdate)
            {
                FaceCamera();
            }
        }

        [Button]
        public void FaceCamera()
        {
            if (TargetCamera == null) { return; }

            var targetRotation = transform.position - TargetCamera.transform.position;
            transform.rotation = Quaternion.LookRotation(targetRotation);
        }
    }
}