using SurvivalTemplatePro.UISystem;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.InputSystem
{
    [Serializable]
    public struct PointerRaycastEventParams
    {
        public GameObject RaycastObject;
        public Vector2 RaycastPosition;
        public ItemSlotUI SlotUI;


        public PointerRaycastEventParams(GameObject raycastObject, Vector2 raycastPosition, ItemSlotUI slotUI)
        {
            this.RaycastObject = raycastObject;
            this.RaycastPosition = raycastPosition;
            this.SlotUI = slotUI;
        }
    }

    [Serializable]
    public struct DragEventParams
    {
        public GameObject DragStartRaycast;
        public GameObject CurrentRaycast;
        public Vector2 CurrentPointerPosition;
        public bool SplitItemStack;


        public DragEventParams(GameObject dragStartRaycast, GameObject currentRaycast, Vector2 currentPointerPosition, bool splitItemStack)
        {
            this.DragStartRaycast = dragStartRaycast;
            this.CurrentRaycast = currentRaycast;
            this.CurrentPointerPosition = currentPointerPosition;
            this.SplitItemStack = splitItemStack;
        }
    }

    [Serializable]
    public class SlotEvent : UnityEvent<ItemSlotUI> { }

    [Serializable]
    public class DragEvent : UnityEvent<DragEventParams> { }

    [Serializable]
    public class PointerRaycastEvent : UnityEvent<PointerRaycastEventParams> { }
}
