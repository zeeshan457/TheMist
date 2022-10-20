using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class InputActionsBlocker : MonoBehaviour
    {
        [SerializeField, ReorderableList(HasLabels = false)]
        private InputActionReference[] m_ActionsToBlock;


        public void AddInputBlocker()
        {
            for (int i = 0; i < m_ActionsToBlock.Length; i++)
                InputBlockManager.AddInputBlocker(m_ActionsToBlock[i]);
        }

        public void RemoveInputBlocker()
        {
            for (int i = 0; i < m_ActionsToBlock.Length; i++)
                InputBlockManager.RemoveInputBlocker(m_ActionsToBlock[i]);
        }
    }
}