using UnityEngine;

namespace SurvivalTemplatePro
{
    [System.Serializable]
    public abstract class STPEventListenerBehaviour
    {
        [SerializeField, STPEvent, ReorderableList(elementLabel: "Event")]
        private string[] m_EventNames;


        public void HookToEvent(ISTPEventHandler eventHandler)
        {
            for (int i = 0; i < m_EventNames.Length; i++)
            {
                if (eventHandler.TryGetEventWithName(m_EventNames[i], out STPEvent stpEvent))
                    stpEvent.onActionTriggered += OnActionTriggered;
            }
        }

        protected abstract void OnActionTriggered(float value);
    }
}