using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/object-carry#player-object-carry-input-behaviour")]
    public class PlayerObjectCarryInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_AltInteractInput;

        private IObjectCarryController m_ObjectCarryController;


        public override void OnInitialized()
        {
            GetModule(out m_ObjectCarryController);

            if (m_EnableOnStart)
                m_AltInteractInput.action.Enable();
        }

        private void OnEnable() => m_AltInteractInput.action.started += OnUseCarriedObjectPerformed;
        private void OnDisable() => m_AltInteractInput.action.started -= OnUseCarriedObjectPerformed;

        private void OnUseCarriedObjectPerformed(InputAction.CallbackContext obj) => m_ObjectCarryController.UseCarriedObject();
    }
}