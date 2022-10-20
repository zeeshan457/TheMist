using UnityEngine;

namespace SurvivalTemplatePro
{
    public class HoverInfo
	{
		public bool IsInteractable => (Interactable != null && Interactable.InteractionEnabled);

		public readonly Collider Collider;
		public readonly IInteractable Interactable;


		public HoverInfo(Collider collider, IInteractable interactable)
		{
			Collider = collider;
			Interactable = interactable;
		}
    }
}