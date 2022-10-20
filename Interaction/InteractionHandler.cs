using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/interaction#interaction-handler-module")]
    public class InteractionHandler : CharacterBehaviour, IInteractionHandler
    {
        public HoverInfo HoverInfo => m_HoverInfo;
        public float HoveredObjectDistance => m_HoveredObjectDistance;
        public float InteractProgress
        {
            get => m_InteractionProgress;
            set
            {
                m_InteractionProgress = value;
                onInteractProgressChanged?.Invoke(m_InteractionProgress);
            }
        }

        public event UnityAction<IInteractable> onInteract;
        public event UnityAction<HoverInfo> onHoverInfoChanged;
        public event UnityAction<float> onInteractProgressChanged;

        [SerializeField]
        [Tooltip("The transform used in raycasting.")]
        private Transform m_View;

        [Title("Settings")]

        [SerializeField, Range(0.01f, 25f)]
        [Tooltip("The raycast max distance, anything further away will be ignored.")]
        private float m_RaycastDistance = 2.5f;

        [SerializeField, Range(0.01f, 5f)]
        [Tooltip("The raycast radius, if you're not looking directly at an object you can still interact with that said object if it's in the given radius.")]
        private float m_RaycastRadius = 0.15f;

        [SerializeField]
        [Tooltip("The trigger colliders interaction mode.")]
        private QueryTriggerInteraction m_TriggerInteraction = QueryTriggerInteraction.Ignore;

        [SerializeField]
        [Tooltip("Interaction layer mask, everything this handler can 'see'.")]
        private LayerMask m_LayerMask;

        private float m_HoveredObjectDistance = 10f;
        private HoverInfo m_LastHoveredInfo;
        private HoverInfo m_HoverInfo;

        private Coroutine m_DelayedInteractCoroutine;
        private IInteractable m_Interactable;
        private float m_InteractionProgress;

        private IPauseHandler m_PauseHandler;
        private bool m_Paused;


        public override void OnInitialized()
        {
            if (TryGetModule(out m_PauseHandler))
            {
                m_PauseHandler.onPause += OnPause;
                m_PauseHandler.onUnpause += OnUnpause;
            }
        }

        private void OnPause()
        {
            EndInteraction();
            m_HoverInfo = null;
            UpdateHovering();
            m_Paused = true;
        }

        private void OnUnpause()
        {
            m_Paused = false;
        }

        #region Interaction
        public void StartInteraction()
        {
            if (m_Interactable != null)
                return;

            if (m_HoverInfo != null && m_HoverInfo.IsInteractable)
            {
                m_Interactable = m_HoverInfo.Interactable;

                if (m_Interactable.HoldDuration > 0.01f)
                    m_DelayedInteractCoroutine = StartCoroutine(C_DelayedInteraction());
                else
                    Interact();
            }
        }

        public void EndInteraction()
        {
            if (m_Interactable == null)
                return;

            if (m_DelayedInteractCoroutine != null)
                StopCoroutine(m_DelayedInteractCoroutine);

            InteractProgress = 0f;
            m_Interactable = null;
        }

        private void Interact()
        {
            if (m_Interactable == null)
                return;

            m_Interactable.OnInteract(Character);
            onInteract?.Invoke(m_Interactable);
        }

        private IEnumerator C_DelayedInteraction()
        {
            float endTime = Time.time + m_Interactable.HoldDuration;

            while (Time.time < endTime)
            {
                InteractProgress = 1 - ((endTime - Time.time) / m_Interactable.HoldDuration);

                yield return null;
            }

            Interact();
            EndInteraction();
        }
        #endregion

        #region Detection
        private void Update()
        {
            if (!IsInitialized || !Character.HealthManager.IsAlive || m_Paused)
                return;

            m_LastHoveredInfo = m_HoverInfo;

            UpdateDetection();
            UpdateHovering();
        }

        private void UpdateHovering()
        {
            if (m_HoverInfo != m_LastHoveredInfo)
            {
                // Hover Start
                if (m_HoverInfo != null && m_HoverInfo.IsInteractable)
                    m_HoverInfo.Interactable.OnHoverStart(Character);

                // Hover End
                if (m_LastHoveredInfo != null && m_LastHoveredInfo.IsInteractable)
                    m_LastHoveredInfo.Interactable.OnHoverEnd(Character);

                // Force End Interaction
                if (m_Interactable != null)
                    EndInteraction();

                onHoverInfoChanged?.Invoke(m_HoverInfo);
            }
        }

        private void UpdateDetection()
        {
            Ray ray = new Ray(m_View.transform.position, m_View.transform.forward);

            if (TryDetectObject(ray, out RaycastHit hitInfo))
            {
                m_HoveredObjectDistance = hitInfo.distance;
                var hitCollider = hitInfo.collider;

                if (m_LastHoveredInfo == null || (m_LastHoveredInfo.Collider != hitCollider))
                    m_HoverInfo = new HoverInfo(hitCollider, hitCollider.GetComponent<IInteractable>());
            }
            else
            {
                m_HoveredObjectDistance = 10f;
                m_HoverInfo = null;
            }
        }

        private bool TryDetectObject(Ray ray, out RaycastHit hitInfo)
        {
            bool hitSomething = Physics.Raycast(ray, out hitInfo, m_RaycastDistance, m_LayerMask, QueryTriggerInteraction.Ignore);

            if (!hitSomething)
                hitSomething = Physics.SphereCast(ray, m_RaycastRadius, out hitInfo, m_RaycastDistance, m_LayerMask, m_TriggerInteraction);

            return hitSomething;
        }
        #endregion
    }
}