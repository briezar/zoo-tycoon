using GameDevKit;
using UnityEngine;
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

        protected override void OnValidAwake()
        {
            _inputActions = new();
        }

        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();

        public static void Enable_PlayerMovement(bool enable)
        {
            if (!IsValid) { return; }

            if (enable) { PlayerMovement.Enable(); }
            else { PlayerMovement.Disable(); }
        }

        public static void Enable_UI(bool enable)
        {
            if (!IsValid) { return; }

            if (enable) { UI.Enable(); }
            else { UI.Disable(); }
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