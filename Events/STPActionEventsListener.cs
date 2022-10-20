using System;

namespace SurvivalTemplatePro
{
    [Serializable]
    public class STPActionEventsListener : STPEventListenerBehaviour
    {
        public event Action onActionTriggered;


        protected override void OnActionTriggered(float value) => onActionTriggered?.Invoke();
    }
}