using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IInteractable
    {
        bool InteractionEnabled { get; set; }
        string InteractionText { get; }
        string DescriptionText { get; }
        float HoldDuration { get; }

        event UnityAction onInteracted;
        event UnityAction onDescriptionTextChanged;
        event UnityAction<bool> onInteractionEnabledChanged;


        /// <summary>
        /// Called when a character starts looking at this object.
        /// </summary>
        void OnHoverStart(ICharacter character);

        /// <summary>
        /// Called when a character stops looking at this object.
        /// </summary>
        void OnHoverEnd(ICharacter character);

        /// <summary>
        /// Called when a character interacts with this object.
        /// </summary>
        void OnInteract(ICharacter character);

        #region Monobehaviour
        GameObject gameObject { get; }
        Transform transform { get; }
        #endregion
    }
}