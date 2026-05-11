using System.Collections.Generic;
using GameDevKit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InputSystem_Actions;

namespace ZooTycoon.Input
{
    public class InputManager : SingletonBehaviour<InputManager>
    {
        public static InputSystem_Actions InputActions
        {
            get
            {
                EnsureInstance();
                return _instance._inputActions;
            }
        }

        public static PlayerMovementActions PlayerMovement => InputActions.PlayerMovement;
        public static UIActions UI => InputActions.UI;

        public static bool IsValid => _instance != null;

        private InputSystem_Actions _inputActions;

        private static readonly List<RaycastResult> _raycastResults = new();
        private static readonly Dictionary<EventSystem, PointerEventData> _pointerEventDataLookup = new();

        private static readonly UsageCounter _playerMovementEnableCounter = new();
        private static readonly UsageCounter _uiEnableCounter = new();

        protected override void OnValidAwake()
        {
            _inputActions = new();
        }

        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();

        public static bool IsPointerOverUI()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return false; }
            if (!_pointerEventDataLookup.TryGetValue(eventSystem, out var pointerEventData))
            {
                pointerEventData = new(eventSystem);
                _pointerEventDataLookup[eventSystem] = pointerEventData;
            }
            pointerEventData.position = Pointer.current.position.ReadValue();

            _raycastResults.Clear();
            eventSystem.RaycastAll(pointerEventData, _raycastResults);
            return _raycastResults.Exists(r => r.module is GraphicRaycaster);
        }

        public static void Enable_PlayerMovement(bool enable)
        {
            if (!IsValid) { return; }

            // Increase usage when enable==false
            var isUsing = _playerMovementEnableCounter.Use(!enable, out var changed);
            if (!changed) { return; }

            if (isUsing) { PlayerMovement.Disable(); }
            else { PlayerMovement.Enable(); }
        }

        public static void Enable_UI(bool enable)
        {
            if (!IsValid) { return; }

            var isUsing = _uiEnableCounter.Use(!enable, out var changed);
            if (!changed) { return; }

            if (isUsing) { UI.Disable(); }
            else { UI.Enable(); }
        }

        private static void EnsureInstance()
        {
            if (_instance != null) { return; }
            var go = new GameObject("Input Manager", typeof(InputManager));
            Debug.Log("Auto-created Input Manager");
            DontDestroyOnLoad(go);
        }
    }
}