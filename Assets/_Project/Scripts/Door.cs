using GameDevKit;
using UnityEngine;
using UnityEngine.Events;

namespace ZooTycoon
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _openAnim, _closeAnim;

        private AnimationHash _openAnimHash, _closeAnimHash;

        public UnityEvent OnOpenDoor, OnCloseDoor;

        private void Start()
        {
            _openAnimHash = _openAnim.name;
            _closeAnimHash = _closeAnim.name;
        }

        public void Open()
        {
            OnOpenDoor?.Invoke();
            _animator.Play(_openAnimHash);
        }

        public void Close()
        {
            OnCloseDoor?.Invoke();
            _animator.Play(_closeAnimHash);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DoorInteractable interactable) && interactable.AutoInteract)
            {
                Open();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out DoorInteractable interactable) && interactable.AutoInteract)
            {
                Close();
            }
        }
    }
}