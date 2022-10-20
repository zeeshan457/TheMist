using SurvivalTemplatePro.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.MovementSystem
{
    [DisallowMultipleComponent]
    public class PlayerMotionInput : MonoBehaviour, ICharacterMotionInputHandler, ISaveableComponent
    {
        public Vector2 RawMovementInput => m_MovementInputValue;
        public Vector3 MovementInput => Vector3.ClampMagnitude(m_Motor.transform.TransformVector(new Vector3(m_MovementInputValue.x, 0f, m_MovementInputValue.y)), 1f);
        public bool RunInput => m_RunInputValue;
        public bool CrouchInput => m_CrouchInputValue;
        public bool JumpInput => m_JumpInputValue;

        [SerializeField]
        private bool m_EnableActionsOnStart = true;

        [Title(label: "Actions")]

        [SerializeField]
        private InputActionReference m_MoveInput;

        [Space(3f)]

        [SerializeField]
        private PlayerInputType m_RunType;

        [SerializeField]
        private InputActionReference m_RunInput;

        [Space(3f)]

        [SerializeField]
        private PlayerInputType m_CrouchType;

        [SerializeField]
        private InputActionReference m_CrouchInput;

        [Space(3f)]

        [SerializeField]
        private PlayerInputType m_JumpType;

        [SerializeField, ShowIf("m_JumpType", (int)PlayerInputType.Toggle)]
        private float m_JumpReleaseDelay = 0.05f;

        [SerializeField]
        private InputActionReference m_JumpInput;

        private ICharacterMotor m_Motor;
        private Vector2 m_MovementInputValue;
        private bool m_RunInputValue;
        private bool m_CrouchInputValue;
        private bool m_JumpInputValue;
        private float m_ReleaseJumpBtnTime;


        public void TickInput()
        {
            // Handle movement input.
            m_MovementInputValue = m_MoveInput.action.ReadValue<Vector2>();

            // Handle run input.
            if (m_RunType == PlayerInputType.Hold)
                m_RunInputValue = m_RunInput.action.ReadValue<float>() > 0.1f;
            else
            {
                if (m_RunInput.action.triggered)
                    m_RunInputValue = !m_RunInputValue;
            }

            // Handle crouch input.
            if (m_CrouchType == PlayerInputType.Hold)
                m_CrouchInputValue = m_CrouchInput.action.ReadValue<float>() > 0.1f;
            else
            {
                if (m_CrouchInput.action.triggered)
                    m_CrouchInputValue = !m_CrouchInputValue;
            }

            // Handle jump input.
            if (m_JumpType == PlayerInputType.Hold)
            {
                m_JumpInputValue = m_JumpInput.action.ReadValue<float>() > 0.1f;
            }
            else if (Time.time > m_ReleaseJumpBtnTime || !m_JumpInputValue)
            {
                m_JumpInputValue = m_JumpInput.action.triggered;

                if (m_JumpInputValue)
                    m_ReleaseJumpBtnTime = Time.time + m_JumpReleaseDelay;
            }
        }

        public void ResetAllInputs() 
        {
            m_CrouchInputValue = false;
            m_RunInputValue = false;
            m_JumpInputValue = false;
            m_ReleaseJumpBtnTime = 0f;
        }

        public void UseCrouchInput() => m_CrouchInputValue = false;
        public void UseRunInput() => m_RunInputValue = false;
        public void UseJumpInput() => m_JumpInputValue = false;

        private void Start()
        {
            if (m_EnableActionsOnStart)
            {
                m_MoveInput.action.Enable();
                m_RunInput.action.Enable();
                m_CrouchInput.action.Enable();
                m_JumpInput.action.Enable();
            }

            m_Motor = GetComponentInParent<ICharacterMotor>();
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_CrouchInputValue = (bool)members[0];
            m_RunInputValue = (bool)members[1];
        }

        public object[] SaveMembers()
        {
            return new object[]
            {
                m_CrouchInputValue,
                m_RunInputValue
            };
        }
        #endregion
    }
}