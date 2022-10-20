using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class InputBlockManager
    {
        #region Internal
        private class InputBlocker
        {
            private InputAction m_InputAction;
            private int m_BlockersCount;


            public InputBlocker(InputAction action)
            {
                this.m_InputAction = action;
            }

            public void DecrementBlockers()
            {
                m_BlockersCount = Mathf.Max(m_BlockersCount - 1, 0);

                if (m_BlockersCount == 0 && !m_InputAction.enabled)
                    m_InputAction.Enable();
            }

            public void IncrementBlockers()
            {
                ++m_BlockersCount;

                if (m_InputAction.enabled)
                    m_InputAction.Disable();
            }

            public void RemoveAllBlockers() 
            {
                m_BlockersCount = 0;

                if (!m_InputAction.enabled)
                    m_InputAction.Enable();
            }
        }
        #endregion

        private readonly static Dictionary<InputActionReference, InputBlocker> m_InputActionBlockers = new Dictionary<InputActionReference, InputBlocker>();


        public static void AddInputBlocker(InputActionReference inputActionRef)
        {
            if (inputActionRef == null)
                return;

            if (m_InputActionBlockers.TryGetValue(inputActionRef, out InputBlocker inputBlocker))
                inputBlocker.IncrementBlockers();
            else
            {
                inputBlocker = new InputBlocker(inputActionRef.action);
                m_InputActionBlockers.Add(inputActionRef, inputBlocker);
                inputBlocker.IncrementBlockers();
            }    
        }

        public static void RemoveInputBlocker(InputActionReference inputActionRef)
        {
            if (inputActionRef == null)
                return;

            if (m_InputActionBlockers.TryGetValue(inputActionRef, out InputBlocker inputBlocker))
                inputBlocker.DecrementBlockers();
        }

        public static void RemoveAllBlockers(InputActionReference inputActionRef)
        {
            if (inputActionRef == null)
                return;

            if (m_InputActionBlockers.TryGetValue(inputActionRef, out InputBlocker inputBlocker))
                inputBlocker.RemoveAllBlockers();
        }

        public static void ClearAllBlockers()
        {
            foreach (var blockers in m_InputActionBlockers.Values)
                blockers.RemoveAllBlockers();
        }
    }
}