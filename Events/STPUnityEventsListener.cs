using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class STPUnityEventsListener : STPEventsListenerBehaviour
    {
        #region Internal
        [System.Serializable]
        public class STPUnityEventListener : STPEventListenerBehaviour
        {
            [Space, SerializeField]
            private FloatEvent m_Event;


            protected override void OnActionTriggered(float value) => m_Event?.Invoke(value);
        }
        #endregion

        [SerializeField]
        [Tooltip("Listens to the parent Event Handler for events and raises the according unity events.")]
        private STPUnityEventListener[] m_Events;


        protected override IEnumerable<STPEventListenerBehaviour> GetEvents() => m_Events;
    }
}