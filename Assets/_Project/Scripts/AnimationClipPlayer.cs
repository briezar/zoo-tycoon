using System.Collections.Generic;
using UnityEngine;

namespace ZooTycoon
{
    public class AnimationClipPlayer : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private readonly Dictionary<AnimationClip, int> _clipHashLookup = new();

        public void Play(AnimationClip clip)
        {
            if (!_clipHashLookup.TryGetValue(clip, out int hash))
            {
                hash = Animator.StringToHash(clip.name);
                _clipHashLookup[clip] = hash;
            }
            _animator.Play(hash);
        }
    }
}