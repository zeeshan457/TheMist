using SurvivalTemplatePro.UISystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurvivalTemplatePro.InputSystem
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/user-interface/behaviours/ui_inventory#player-ui-inventory-input")]
    public class PlayerUIInventoryInput : PlayerUIBehaviour
    {
        [SerializeField]
        private GraphicRaycaster m_Raycaster;

        [SerializeField]
        private EventSystem m_EventSystem;

        [Title("Actions")]

        [SerializeField]
        private InputActionReference m_LeftClickInput;

        [SerializeField]
        private InputActionReference m_RightClickInput;

        [SerializeField]
        private InputActionReference m_PointerPositionInput;

        [SerializeField]
        private InputActionReference m_InventoryToggleInput;

        [SerializeField]
        private InputActionReference m_InventoryCloseInput;

        [SerializeField]
        private InputActionReference m_AutoMoveItemInput;

        [SerializeField]
        private InputActionReference m_SplitItemStackInput;

        [Title("Events")]

        [SerializeField]
        private SlotEvent m_AutoMoveItemCallback;

        [SerializeField]
        private DragEvent m_DragStartCallback;

        [SerializeField]
        private DragEvent m_DragCallback;

        [SerializeField]
        private DragEvent m_DragEndCallback;

        [SerializeField]
        private PointerRaycastEvent m_PointerMovedCallback;

        [SerializeField]
        private UnityEvent m_LeftClickCallback;

        [SerializeField]
        private UnityEvent m_RightClickCallback;

        private PointerEventData m_PointerEventData;
        private List<RaycastResult> m_RaycastResults = new List<RaycastResult>();
        private GameObject m_DragStartRaycast;
        private bool m_IsDragging;
        private bool m_PointerMovedLastFrame = false;
        
        private bool m_SubscribedToInputEvents;

        private Vector2 m_PointerPositionLastFrame;

        private IInventoryInspectManager m_InventoryInspector;


        public override void OnAttachment()
        {
            GetModule(out m_InventoryInspector);

            m_PointerEventData = new PointerEventData(m_EventSystem);

            m_AutoMoveItemInput.action.actionMap.Enable();
            m_PointerPositionInput.action.actionMap.Enable();
            m_PointerPositionLastFrame = m_PointerPositionInput.action.ReadValue<Vector2>();

            m_InventoryToggleInput.action.started += OnInventoryToggleInput;
            m_InventoryInspector.onInspectStarted += OnInventoryInspection;
            m_InventoryInspector.onInspectEnded += UnsubscribeFromInputEvents;
        }

        public override void OnDetachment()
        {
            if (m_SubscribedToInputEvents)
                UnsubscribeFromInputEvents();

            m_InventoryToggleInput.action.started -= OnInventoryToggleInput;
            m_InventoryInspector.onInspectStarted -= OnInventoryInspection;
            m_InventoryInspector.onInspectEnded -= UnsubscribeFromInputEvents;
        }

        private void OnInventoryInspection(InventoryInspectState state) => SubscribeToInputEvents();

        private void SubscribeToInputEvents() 
        {
            if (m_SubscribedToInputEvents)
                return;

            m_InventoryCloseInput.action.started += OnInventoryCloseInput;
            m_AutoMoveItemInput.action.performed += OnAutoMoveItemInput;
            m_LeftClickInput.action.performed += OnLeftClick;
            m_RightClickInput.action.performed += OnRightClick;

            m_SubscribedToInputEvents = true;
        }

        private void UnsubscribeFromInputEvents() 
        {
            if (!m_SubscribedToInputEvents)
                return;

            m_InventoryCloseInput.action.started -= OnInventoryCloseInput;
            m_AutoMoveItemInput.action.performed -= OnAutoMoveItemInput;
            m_LeftClickInput.action.performed -= OnLeftClick;
            m_RightClickInput.action.performed -= OnRightClick;

            m_SubscribedToInputEvents = false;
        }

        #region Input Events

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            if (context.action.WasReleasedThisFrame())
                m_LeftClickCallback?.Invoke();
        }

        private void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.action.WasReleasedThisFrame())
                m_RightClickCallback?.Invoke();
        }

        private void OnInventoryToggleInput(InputAction.CallbackContext context)
        {
            if (m_InventoryInspector.InspectState == InventoryInspectState.None)
                m_InventoryInspector.TryInspect(InventoryInspectState.Default);
            else
                m_InventoryInspector.TryStopInspecting();
        }

        private void OnInventoryCloseInput(InputAction.CallbackContext obj)
        {
            if (m_InventoryInspector.InspectState != InventoryInspectState.None)
                m_InventoryInspector.TryStopInspecting();
        }

        private void OnAutoMoveItemInput(InputAction.CallbackContext context)
        {
            if (m_InventoryInspector.InspectState == InventoryInspectState.None)
                return;

            var currentRaycast = GetCurrentRaycast();

            if (currentRaycast != null && currentRaycast.TryGetComponent(out ItemSlotUI slot))
                m_AutoMoveItemCallback.Invoke(slot);
        }

        #endregion

        #region Input Checking

        public override void OnInterfaceUpdate()
        {
            if (m_InventoryInspector.InspectState == InventoryInspectState.None || !Player.HealthManager.IsAlive)
                return;

            Vector2 pointerPosition = m_PointerPositionInput.action.ReadValue<Vector2>();
            bool pointerMovedThisFrame = UpdatePointerPosition(pointerPosition);

            UpdateDragging(pointerPosition, pointerMovedThisFrame, m_PointerMovedLastFrame);

            m_PointerMovedLastFrame = pointerMovedThisFrame;
        }

        /// <returns> TRUE if the pointer moved, FALSE otherwise. </returns>
        private bool UpdatePointerPosition(Vector2 pointerPosition)
        {
            bool pointerMovedThisFrame = (pointerPosition - m_PointerPositionLastFrame).sqrMagnitude > 0.01f;       
            m_PointerPositionLastFrame = pointerPosition;

            if (pointerMovedThisFrame)
            {
                Raycast();

                GameObject currentRaycast = GetCurrentRaycast();
                ItemSlotUI itemSlot = currentRaycast != null ? currentRaycast.GetComponent<ItemSlotUI>() : null;

                m_PointerMovedCallback?.Invoke(new PointerRaycastEventParams(currentRaycast, pointerPosition, itemSlot));
            }

            return pointerMovedThisFrame;
        }

        private void UpdateDragging(Vector2 pointerPosition, bool pointerMovedThisFrame, bool pointerMovedLastFrame)
        {
            var leftClick = m_LeftClickInput.action;

            if (!m_IsDragging && leftClick.ReadValue<float>() > 0.1f && pointerMovedThisFrame && pointerMovedLastFrame)
            {
                m_IsDragging = true;

                m_DragStartRaycast = GetCurrentRaycast();
                bool splitItemStack = m_SplitItemStackInput.action.ReadValue<float>() == 1f;
                m_DragStartCallback.Invoke(new DragEventParams(m_DragStartRaycast, m_DragStartRaycast, pointerPosition, splitItemStack));
            }
            else if (m_IsDragging && leftClick.WasReleasedThisFrame())
            {
                m_IsDragging = false;

                bool splitItemStack = m_SplitItemStackInput.action.ReadValue<float>() == 1f;
                m_DragEndCallback.Invoke(new DragEventParams(m_DragStartRaycast, GetCurrentRaycast(), pointerPosition, splitItemStack));
            }
            else if (m_IsDragging )//&& (!leftClick.WasPerformedThisFrame() || leftClick.WasReleasedThisFrame()))
            {
                bool splitItemStack = m_SplitItemStackInput.action.ReadValue<float>() == 1f;
                m_DragCallback.Invoke(new DragEventParams(m_DragStartRaycast, GetCurrentRaycast(), pointerPosition, splitItemStack));
            }
        }

        private void Raycast()
        {
            // Set the Pointer Event Position to that of the game object
            m_PointerEventData.position = m_PointerPositionInput.action.ReadValue<Vector2>();

            m_RaycastResults.Clear();

            // Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, m_RaycastResults);
        }

        private GameObject GetCurrentRaycast() => m_RaycastResults.Count > 0 ? m_RaycastResults[0].gameObject : null;

        #endregion
    }
}