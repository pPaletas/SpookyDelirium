using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.FPS.Gameplay
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        GameFlowManager m_GameFlowManager;
        PlayerCharacterController m_PlayerCharacterController;
        PlayerInput m_Input;
        bool m_FireInputWasHeld;

        void Start()
        {
            m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerInputHandler>(
                m_PlayerCharacterController, this, gameObject);
            m_GameFlowManager = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, PlayerInputHandler>(m_GameFlowManager, this);

            m_Input = GetComponent<PlayerInput>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void LateUpdate()
        {
            m_FireInputWasHeld = GetFireInputHeld();
        }

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked && !m_GameFlowManager.GameIsEnding;
        }

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                Vector2 input = m_Input.actions["Walk"].ReadValue<Vector2>();
                Vector3 move = new Vector3(input.x, 0f, input.y);

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseOrStickLookAxis("Horizontal");
        }

        public float GetLookInputsVertical()
        {
            return GetMouseOrStickLookAxis("Vertical");
        }

        public bool GetDashInputDown()
        {
            if (CanProcessInput())
            {
                return m_Input.actions["Dash"].WasPressedThisFrame();
            }

            return false;
        }

        public bool GetJumpInputDown()
        {
            if (CanProcessInput())
            {
                return m_Input.actions["Jump"].WasPressedThisFrame();
            }

            return false;
        }

        public bool GetJumpInputHeld()
        {
            if (CanProcessInput())
            {
                return m_Input.actions["Jump"].IsPressed();
            }

            return false;
        }

        public bool GetFireInputDown()
        {
            return GetFireInputHeld() && !m_FireInputWasHeld;
        }

        public bool GetFireInputReleased()
        {
            return !GetFireInputHeld() && m_FireInputWasHeld;
        }

        public bool GetFireInputHeld()
        {
            if (CanProcessInput())
            {
                return m_Input.actions["Shoot"].IsPressed();
            }

            return false;
        }

        public bool GetAimInputHeld()
        {
            return false;
        }

        public bool GetSprintInputHeld()
        {
            if (CanProcessInput())
            {
                return m_Input.actions["Run"].IsPressed();
            }

            return false;
        }

        public bool GetCrouchInputDown()
        {
            return false;
        }

        public bool GetCrouchInputReleased()
        {
            return false;
        }

        public bool GetReloadButtonDown()
        {
            if (CanProcessInput())
            {
                return m_Input.actions["Reload"].WasPressedThisFrame();
            }

            return false;
        }

        public int GetSwitchWeaponInput()
        {
            if (CanProcessInput())
            {

                float scrollDelta = m_Input.actions["NextWeapon"].ReadValue<Vector2>().y;

                if (scrollDelta > 0f)
                    return -1;
                else if (scrollDelta < 0f)
                    return 1;
            }

            return 0;
        }

        public int GetSelectWeaponInput()
        {
            if (CanProcessInput())
            {
                return (int) m_Input.actions["SelectWeapon"].ReadValue<float>();
            }

            return 0;
        }

        float GetMouseOrStickLookAxis(string axis)
        {
            if (CanProcessInput())
            {
                Vector2 input = m_Input.actions["Look"].ReadValue<Vector2>();
                float i = axis == "Horizontal" ? input.x : input.y;

                // handle inverting vertical input
                if (axis == "Vertical" && InvertYAxis)
                    i *= -1f;
                else if (axis == "Horizontal" && InvertXAxis)
                    i *= -1f;

                // apply sensitivity multiplier
                i *= LookSensitivity;
                i *= Time.deltaTime;

                return i;
            }

            return 0f;
        }
    }
}