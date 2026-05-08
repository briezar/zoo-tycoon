using UnityEngine;

namespace ZooTycoon.Input
{
    public class InputActionMapHandler : MonoBehaviour
    {
        public void Enable_PlayerMovement(bool enable) => InputManager.Enable_PlayerMovement(enable);
        public void Enable_UI(bool enable) => InputManager.Enable_UI(enable);
    }
}