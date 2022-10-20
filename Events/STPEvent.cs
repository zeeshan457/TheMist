using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    [System.Serializable]
    public class STPEvent
    {
        public string EventName => m_EventName;

        public event UnityAction<float> onActionTriggered;

        [SerializeField]
        private string m_EventName;


        public STPEvent(string name)
        {
            this.m_EventName = name;
        }

        public void TriggerAction(float value) => onActionTriggered?.Invoke(value);
    }
}