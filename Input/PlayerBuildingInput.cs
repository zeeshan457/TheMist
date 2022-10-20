using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/building#player-building-input-behaviour")]
    public class PlayerBuildingInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [Title(label: "Actions")]

        [SerializeField]
        private InputActionReference m_PlaceableCycleRightInput;

        [SerializeField]
        private InputActionReference m_PlaceableCycleLeftInput;

        [SerializeField]
        private InputActionReference m_PlaceableRotateInput;

        [SerializeField]
        private InputActionReference m_PlaceInput;

        [SerializeField]
        private InputActionReference m_ExitBuildingModeInput;

        [SerializeField]
        private InputActionReference m_PreviewCancelInput;

        [SerializeField]
        private InputActionReference m_PlaceMaterialInput;

        private IBuildingController m_BuildingController;
        private IStructureDetector m_StructureDetector;
        private IInventoryMaterialsHandler m_InventoryMaterialsHandler;


        public override void OnInitialized()
        {
            GetModule(out m_BuildingController);
            GetModule(out m_StructureDetector);
            GetModule(out m_InventoryMaterialsHandler);

            if (m_EnableOnStart)
            {
                m_PlaceableCycleRightInput.action.Enable();
                m_PlaceableCycleLeftInput.action.Enable();
                m_PlaceableRotateInput.action.Enable();
                m_PlaceInput.action.Enable();
                m_ExitBuildingModeInput.action.Enable();
                m_PreviewCancelInput.action.Enable();
                m_PlaceMaterialInput.action.Enable();
            }
        }

        private void OnEnable()
        {
            m_PlaceableCycleRightInput.action.started += OnCycleRightPerformed;
            m_PlaceableCycleLeftInput.action.started += OnCycleLeftPerformed;
            m_PlaceableRotateInput.action.performed += OnPlaceableRotate;
            m_PlaceInput.action.started += OnPlacePerformed;
            m_ExitBuildingModeInput.action.started += OnExitBuildingModePerformed;
            m_PreviewCancelInput.action.started += CancelPreview;
            m_PlaceMaterialInput.action.started += OnPlaceMaterial;
        }

        private void OnDisable()
        {
            m_PlaceableCycleRightInput.action.started -= OnCycleRightPerformed;
            m_PlaceableCycleLeftInput.action.started -= OnCycleLeftPerformed;
            m_PlaceableRotateInput.action.performed -= OnPlaceableRotate;
            m_PlaceInput.action.started -= OnPlacePerformed;
            m_ExitBuildingModeInput.action.started -= OnExitBuildingModePerformed;
            m_PreviewCancelInput.action.started -= CancelPreview;
            m_PlaceMaterialInput.action.started -= OnPlaceMaterial;
        }

        private void OnExitBuildingModePerformed(InputAction.CallbackContext obj) => m_BuildingController.EndBuilding();
        private void OnPlacePerformed(InputAction.CallbackContext obj) => m_BuildingController.PlaceObject();

        private void OnPlaceableRotate(InputAction.CallbackContext obj) => m_BuildingController.RotateObject(obj.ReadValue<Vector2>().y / 120f);

        private void OnCycleLeftPerformed(InputAction.CallbackContext obj) => m_BuildingController.SelectNextPlaceable(false);
        private void OnCycleRightPerformed(InputAction.CallbackContext obj) => m_BuildingController.SelectNextPlaceable(true);

        private void CancelPreview(InputAction.CallbackContext obj) => m_StructureDetector.CancelStructureInView();
        private void OnPlaceMaterial(InputAction.CallbackContext obj) => m_InventoryMaterialsHandler.AddMaterial(m_StructureDetector.StructureInView);
    }
}