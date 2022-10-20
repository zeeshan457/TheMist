using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Deals with the Player death and respawn behaviour.
    /// </summary>
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/health#player-death-handler-behaviour")]
    public class PlayerDeathHandler : CharacterBehaviour
    {
        private enum ItemDropType { All, Equipped, None }

        [InfoBox("Items to drop on death.")]
        [SerializeField]
        private ItemDropType m_ItemDropType = ItemDropType.None;


        public override void OnInitialized()
        {
            Character.HealthManager.onDeath += OnPlayerDeath;
            Character.HealthManager.onRespawn += OnPlayerRespawn;
        }

        private void OnPlayerDeath()
        {
            // Pause the player
            if (TryGetModule(out IPauseHandler pauseHandler))
                pauseHandler.RegisterLocker(this, new PlayerPauseParams(true, true, true, true));

            // Disable the Character Controller
            CharacterController characterController = Character.gameObject.GetComponent<CharacterController>();
            characterController.enabled = false;

            // Stop inventory inspection
            if (TryGetModule(out IInventoryInspectManager inventoryInspectManager))
                inventoryInspectManager.TryStopInspecting();

            // Handle item dropping
            if (TryGetModule(out IItemDropHandler itemDropHandler))
                HandleItemDrop(itemDropHandler);

            // Holster Weapon
            if (TryGetModule(out IWieldablesController wieldablesController))
                wieldablesController.TryEquipWieldable(null, 1.5f);

            // Do death module effects
            if (TryGetModule(out IDeathModule deathModule))
                deathModule.DoDeathEffects(Character);
        }

        private void OnPlayerRespawn() 
        {
            // Unpause the player
            if (TryGetModule(out IPauseHandler pauseHandler))
                pauseHandler.UnregisterLocker(this);

            // Do death module respawn effects
            if (TryGetModule(out IDeathModule deathModule))
                deathModule.DoRespawnEffects(Character);

            // Reset Thirst
            if (TryGetModule(out IThirstManager thirst))
                thirst.Thirst = thirst.MaxThirst;

            // Reset Energy
            if (TryGetModule(out IEnergyManager energy))
                energy.Energy = energy.MaxEnergy;

            // Reset Hunger
            if (TryGetModule(out IHungerManager hunger))
                hunger.Hunger = hunger.MaxHunger;

            StartCoroutine(C_EnableController());
        }

        private IEnumerator C_EnableController() 
        {
            yield return new WaitForEndOfFrame();

            // Re-enable the Character Controller
            CharacterController characterController = Character.gameObject.GetComponent<CharacterController>();
            characterController.enabled = true;

            // Reset the player's state
            if (TryGetModule(out IMotionController motionController))
                motionController.ResetController();
        }

        private void DropAllItems(IInventory inventory, IItemDropHandler dropHandler)
        {
            for (int i = 0; i < inventory.Containers.Count; i++)
            {
                for (int j = 0; j < inventory.Containers[i].Slots.Length; j++)
                {
                    var slot = inventory.Containers[i].Slots[j];

                    if (slot.HasItem)
                        dropHandler.DropItem(slot);
                }
            }
        }

        private void HandleItemDrop(IItemDropHandler dropHandler) 
        {
            switch (m_ItemDropType)
            {
                // Drop all inventory items
                case ItemDropType.All:
                    DropAllItems(Character.Inventory, dropHandler);
                    break;

                // Drop the Holstered wieldable
                case ItemDropType.Equipped:
                    if (m_ItemDropType == ItemDropType.Equipped)
                    {
                        if (TryGetModule(out IWieldablesController wieldablesController) && wieldablesController.ActiveWieldable != null)
                        {
                            var attachedItem = wieldablesController.ActiveWieldable.AttachedItem;

                            if (attachedItem != null)
                                dropHandler.DropItem(attachedItem, 0f);
                        }
                    }
                    break;

                // Don't drop anything
                case ItemDropType.None:
                    break;
            }
        }
    }
}
