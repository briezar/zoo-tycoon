using GameDevKit;
using UnityEngine;

namespace ZooTycoon
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SerializableAnimationHash _moveSpeedParam;
        [SerializeField] private SerializableAnimationHash _attackAnim;

        public void SyncMoveAnim(float currentSpeed, float walkSpeed)
        {
            var lerpedSpeed = MathUtils.UnclampedInverseLerp(0, walkSpeed, currentSpeed);
            _animator.SetFloat(_moveSpeedParam, lerpedSpeed);
        }

        public void PlayAttackAnim()
        {
            _animator.Play(_attackAnim, 1);
        }
    }
}