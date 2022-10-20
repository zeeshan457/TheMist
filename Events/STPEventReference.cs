using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [Serializable]
    public class STPEventReference
    {
        public string Name => m_EventName;

#if UNITY_EDITOR
        public bool SetToExistingIfAvailable => m_SetToExistingIfAvailable;
        public string DefaultName => m_DefaultName;
#endif

        [SerializeField, EnableIf("", true)]
        private string m_EventName;

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private bool m_SetToExistingIfAvailable;

        [SerializeField, EnableIf("", true)]
        private string m_DefaultName;
#endif


        public STPEventReference(string name, bool setToExistingIfAvailable = true)
        {
            this.m_EventName = name;

#if UNITY_EDITOR
            this.m_SetToExistingIfAvailable = setToExistingIfAvailable;
            this.m_DefaultName = m_EventName;
#endif
        }

#if UNITY_EDITOR
        public void SetName(string name) => m_EventName = name;
#endif
    }
}