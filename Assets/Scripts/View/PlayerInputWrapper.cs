using UnityEngine;
using UnityEngine.InputSystem;

namespace Erogemy.BlockBreaker.View
{
    public class PlayerInputWrapper : MonoBehaviour
    {
        [SerializeField] PlayerInput playerInput;

        InputAction touchPositionAction;
        public Vector2 TouchScreenPosition { get; private set; }

        void Awake()
        {
            touchPositionAction = playerInput.actions["TouchPosition"];
        }

        void Update()
        {
            TouchScreenPosition = touchPositionAction.ReadValue<Vector2>();
        }
    }
}
