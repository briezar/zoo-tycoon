using System.Collections.Generic;
using GameDevKit;
using UnityEngine;
using UnityEngine.Events;

namespace ZooTycoon
{
    public class Door : MonoBehaviour
    {
        public UnityEvent OnOpenDoor, OnCloseDoor;

        public bool IsOpen { get; private set; } = false;

        private readonly Dictionary<Collider, DoorInteractable> _interactableLookup = new();

        public void Open()
        {
            IsOpen = true;
            OnOpenDoor?.Invoke();
        }

        public void Close()
        {
            IsOpen = false;
            OnCloseDoor?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out DoorInteractable interactable)) { return; }
            _interactableLookup[other] = interactable;

            if (IsOpen) { return; }

            if (interactable.AutoInteract)
            {
                Open();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_interactableLookup.Remove(other, out var interactable)) { return; }
            if (_interactableLookup.Count > 0) { return; }

            if (interactable.AutoInteract)
            {
                Close();
            }
        }
    }
}