using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public abstract class STPEventsListenerBehaviour : MonoBehaviour
    {
        protected virtual void Awake() 
        {
            var eventHandler = GetComponentInParent<ISTPEventHandler>();

            if (eventHandler == null)
                return;

            foreach (var stpEvent in GetEvents())
                stpEvent.HookToEvent(eventHandler);
        }

        protected abstract IEnumerable<STPEventListenerBehaviour> GetEvents();
    }
}