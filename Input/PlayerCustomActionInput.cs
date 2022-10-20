using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class PlayerCustomActionInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_CancelActionInput;

        private ICustomActionManager m_CustomActionManager;


        public override void OnInitialized()
        {
            GetModule(out m_CustomActionManager);

            if (m_EnableOnStart)
                m_CancelActionInput.action.Enable();
        }

        private void OnEnable()
        {
            m_CancelActionInput.action.started += OnInteractStart;
        }

        private void OnDisable()
        {
            m_CancelActionInput.action.started -= OnInteractStart;
        }

        private void OnInteractStart(InputAction.CallbackContext context) => m_CustomActionManager.TryCancelAction();     
    }
}