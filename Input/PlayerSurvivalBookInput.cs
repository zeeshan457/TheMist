using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class PlayerSurvivalBookInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [Title(label: "Actions")]

        [SerializeField]
        private InputActionReference m_ToggleSurvivalBook;

        private IWieldableSurvivalBookHandler m_SurvivalBookHandler;


        public override void OnInitialized()
        {
            GetModule(out m_SurvivalBookHandler);

            if (m_EnableOnStart)
                m_ToggleSurvivalBook.action.Enable();
        }

        private void OnEnable() => m_ToggleSurvivalBook.action.started += OnSurvivalBookTogglePerfomed;
        private void OnDisable() => m_ToggleSurvivalBook.action.started -= OnSurvivalBookTogglePerfomed;

        private void OnSurvivalBookTogglePerfomed(InputAction.CallbackContext obj) => m_SurvivalBookHandler.ToggleInspection();
    }
}