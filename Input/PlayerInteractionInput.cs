using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/interaction#player-interaction-input-behaviour")]
    public class PlayerInteractionInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_InteractInput;

        private IInteractionHandler m_InteractionHandler;


        public override void OnInitialized()
        {
            GetModule(out m_InteractionHandler);

            if (m_EnableOnStart)
                m_InteractInput.action.Enable();
        }

        private void OnEnable()
        {
            m_InteractInput.action.started += OnInteractStart;
            m_InteractInput.action.canceled += OnInteractEnd;
        }

        private void OnDisable()
        {
            m_InteractInput.action.started -= OnInteractStart;
            m_InteractInput.action.canceled -= OnInteractEnd;
        }

        private void OnInteractStart(InputAction.CallbackContext context)
        {
            m_InteractionHandler.StartInteraction();
        }

        private void OnInteractEnd(InputAction.CallbackContext obj) => m_InteractionHandler.EndInteraction();
    }
}
