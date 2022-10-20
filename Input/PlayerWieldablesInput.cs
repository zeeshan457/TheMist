using SurvivalTemplatePro.WieldableSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/wieldable#player-wieldables-input-behaviour")]
    public class PlayerWieldablesInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [Title(label: "Actions")]

        [SerializeField]
        private InputActionReference m_UseInput;

        [SerializeField]
        private InputActionReference m_ReloadInput;

        [SerializeField]
        private InputActionReference m_DropInput;

        [SerializeField]
        private PlayerInputType m_AimType;

        [SerializeField]
        private InputActionReference m_AimInput;

        private IWieldablesController m_Controller;
        private IWieldableDropHandler m_DropHandler;

        private IAimHandler m_AimHandler;
        private IUseHandler m_UseHandler;
        private IReloadHandler m_ReloadHandler;


        public override void OnInitialized()
        {
            GetModule(out m_Controller);
            GetModule(out m_DropHandler);

            m_Controller.onWieldableEquipped += OnWieldableChanged;

            if (m_EnableOnStart)
            {
                m_UseInput.action.Enable();
                m_AimInput.action.Enable();
                m_ReloadInput.action.Enable();
                m_DropInput.action.Enable();
            }
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            if (wieldable == null)
            {
                m_AimHandler = null;
                m_UseHandler = null;
                m_ReloadHandler = null;
            }
            else
            {
                m_AimHandler = wieldable.gameObject.GetComponent<IAimHandler>();
                m_UseHandler = wieldable.gameObject.GetComponent<IUseHandler>();
                m_ReloadHandler = wieldable.gameObject.GetComponent<IReloadHandler>();
            }
        }

        private void OnEnable()
        {
            m_DropInput.action.started += OnDropActionPerformed;
            m_ReloadInput.action.started += TryStartReload;
        }

        private void OnDisable()
        {
            m_DropInput.action.started -= OnDropActionPerformed;
            m_ReloadInput.action.started -= TryStartReload;
        }

        private void OnDropActionPerformed(InputAction.CallbackContext obj) => m_DropHandler.DropWieldable();

        private void TryStartReload(InputAction.CallbackContext obj)
        {
            if (m_ReloadHandler != null)
                m_ReloadHandler.StartReloading();
        }

        private void Update()
        {
            if (m_UseHandler != null)
                HandleUseInput();

            if (m_AimHandler != null)
                HandleAimInput();
        }

        private void HandleUseInput()
        {
            if (m_UseInput.action.triggered)
                m_UseHandler.Use(UsePhase.Start);
            else if (m_UseInput.action.ReadValue<float>() > 0.001f)
                m_UseHandler.Use(UsePhase.Hold);
            else if (m_UseInput.action.WasReleasedThisFrame() || !m_UseInput.action.enabled)
                m_UseHandler.Use(UsePhase.End);
        }

        private void HandleAimInput()
        {
            if (m_AimType == PlayerInputType.Hold)
            {
                if (m_AimInput.action.ReadValue<float>() > 0.001f)
                {
                    if (!m_AimHandler.IsAiming)
                        m_AimHandler.StartAiming();
                }
                else if (m_AimHandler.IsAiming)
                    m_AimHandler.EndAiming();

                return;
            }

            if (m_AimType == PlayerInputType.Toggle)
            {
                if (m_AimInput.action.WasPressedThisFrame())
                {
                    if (m_AimHandler.IsAiming)
                        m_AimHandler.EndAiming();
                    else
                        m_AimHandler.StartAiming();
                }

                return;
            }
        }
    }
}