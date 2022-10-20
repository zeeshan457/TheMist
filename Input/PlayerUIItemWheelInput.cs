using SurvivalTemplatePro.UISystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    [RequireComponent(typeof(IItemWheelUI))]
    public class PlayerUIItemWheelInput : PlayerUIBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [SerializeField]
        private InputActionReference m_ItemWheelInput;

        [SerializeField]
        private InputActionReference m_CursorDeltaInput;

        private IItemWheelUI m_ItemWheelUI;


        public override void OnAttachment()
        {
            m_ItemWheelUI = GetComponent<IItemWheelUI>();

            if (m_EnableOnStart)
            {
                m_ItemWheelInput.action.Enable();
                m_CursorDeltaInput.action.Enable();
            }
        }

        private void OnEnable()
        {
            m_ItemWheelInput.action.started += OnItemWheelInput;
            m_ItemWheelInput.action.canceled += OnItemWheelInput;
            m_CursorDeltaInput.action.performed += OnCursorMovedInput;
        }

        private void OnDisable()
        {
            m_ItemWheelInput.action.started -= OnItemWheelInput;
            m_ItemWheelInput.action.canceled -= OnItemWheelInput;
            m_CursorDeltaInput.action.performed -= OnCursorMovedInput;
        }

        private void OnCursorMovedInput(InputAction.CallbackContext obj)
        {
            if (m_ItemWheelUI != null)
                m_ItemWheelUI.UpdateSelection(obj.ReadValue<Vector2>().normalized);
        }

        private void OnItemWheelInput(InputAction.CallbackContext context)
        {
            if (context.started && !m_ItemWheelUI.IsVisible)
                m_ItemWheelUI.StartInspection();
            else if (context.canceled && m_ItemWheelUI.IsVisible)
                m_ItemWheelUI.EndInspection();
        }
    }
}