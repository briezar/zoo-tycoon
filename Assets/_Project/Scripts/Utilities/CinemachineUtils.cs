using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace ZooTycoon
{
    public static class CinemachineUtils
    {
        private static CinemachineCamera _lookCam, _sourceCam;

        public static async UniTask WaitForBlending(CinemachineBrain brain = null, CancellationToken ct = default)
        {
            brain ??= CinemachineBrain.GetActiveBrain(0);
            await UniTask.Yield();
            await UniTask.WaitUntil(() => !brain.IsBlending, cancellationToken: ct).SuppressCancellationThrow();
        }

        public static async UniTask LookAt(Transform target, float lookDuration, CancellationToken ct = default)
        {
            var brain = CinemachineBrain.GetActiveBrain(0);
            if (brain.ActiveVirtualCamera is not CinemachineCamera sourceCam) { return; }

            if (_lookCam == null) { _sourceCam = null; }
            if (_sourceCam != sourceCam)
            {
                _sourceCam = sourceCam;
                _lookCam = UnityEngine.Object.Instantiate(sourceCam);
                _lookCam.gameObject.SetActive(false);

                foreach (var component in _lookCam.GetComponentsInChildren<Component>(true))
                {
                    if (component is CinemachineComponentBase or ICinemachineCamera or Transform) { continue; }
                    UnityEngine.Object.Destroy(component);
                }
            }

            _lookCam.Priority = sourceCam.Priority;

            _lookCam.Follow = target;
            _lookCam.gameObject.SetActive(true);

            try
            {
                await WaitForBlending(brain, ct);
                await UniTask.WaitForSeconds(lookDuration, cancellationToken: ct);

                _lookCam.gameObject.SetActive(false);

                await WaitForBlending(brain, ct);
            }
            catch (OperationCanceledException)
            {
                _lookCam.gameObject.SetActive(false);
            }
        }
    }
}