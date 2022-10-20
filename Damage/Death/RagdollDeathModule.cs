using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class RagdollDeathModule : MonoBehaviour
    {
        [Title("Ragdoll")]

        [SerializeField]
        private Transform m_Camera;

        [SerializeField]
        private Transform m_HeadTransform;

        [SerializeField]
        private Vector3 m_HeadRotOffset;

        [SerializeField]
        private float m_HeadLerpSpeed = 5f;

        private Coroutine m_CameraMover;


        public void DoDisableEffects() => m_CameraMover = StartCoroutine(C_MoveCameraToHeadPosition());
        public void DoEnableEffects() => StopCoroutine(m_CameraMover);

        private IEnumerator C_MoveCameraToHeadPosition()
        {
            while (true)
            {
                m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position, m_HeadTransform.position, Time.deltaTime * m_HeadLerpSpeed);
                m_Camera.transform.rotation = Quaternion.Lerp(m_Camera.transform.rotation, m_HeadTransform.rotation * Quaternion.Euler(m_HeadRotOffset), Time.deltaTime * m_HeadLerpSpeed);

                yield return null;
            }
        }
    }
}
