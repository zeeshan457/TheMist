using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IInteractionHandler : ICharacterModule
    {
        HoverInfo HoverInfo { get; }
        float HoveredObjectDistance { get; }

        /// <summary>
        /// Interaction progress 0 - 1 Range
        /// </summary>
        float InteractProgress { get; }

        event UnityAction<HoverInfo> onHoverInfoChanged;
        event UnityAction<float> onInteractProgressChanged;
        event UnityAction<IInteractable> onInteract;

        void StartInteraction();
        void EndInteraction();
    }
}