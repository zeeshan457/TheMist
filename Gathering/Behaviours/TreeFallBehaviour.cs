using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.ResourceGathering
{
    public class TreeFallBehaviour : GatherableBehaviour
    {
        #region Internal
        protected enum DestroyType
        {
            ThisObject,
            DontDestroy
        }
        #endregion

        [Title("Tree Falling")]

        [SerializeField, Range(1f, 100f)]
        private float m_MaxTimeToFall = 10f;

        [SerializeField, Range(1f, 1000f)]
        private float m_TreeFallForce = 10f;

        [Space]

        [SerializeField]
        private Rigidbody m_FallingTree;

        [SerializeField]
        private Collider m_TreeStump;

        [Space]

        [SerializeField]
        private SoundPlayer m_TreeFallAudio;

        [Title("Tree Impact")]

        [SerializeField, Range(0f, 100f)]
        private float m_TreeImpactLogsForce = 10f;

        [SerializeField, Range(0f, 100f)]
        private float m_LogsGroundOffset = 0.5f;

        [Space]

        [SerializeField]
        private DestroyType m_DestroyType = DestroyType.ThisObject;

        [SerializeField]
        private TriggerEventHandler m_ImpactTrigger;

        [SerializeField]
        private GameObject m_LogsRoot;

        [Space]

        [SerializeField]
        private GameObject m_TreeImpactFX;

        [SerializeField]
        private SoundPlayer m_TreeImpactAudio;

        [Space]

        [SerializeField]
        private CameraShakeInfo m_CameraShake;

        private float m_TimeSinceFallStart = 0f;
        private bool m_IsFalling = false;
        private bool m_HadImpact = false;

        private Vector3 m_HitDirection = Vector3.zero;


        public override void DoHitEffects(DamageInfo damageInfo) { }

        /// <summary>
        /// Start tree fall
        /// </summary>
        /// <param name="dmgInfo"></param>
        public override void DoDestroyEffects(DamageInfo dmgInfo)
        {
            m_HitDirection = dmgInfo.HitDirection;
            StartTreeFall(m_HitDirection);
        }

        private void StartTreeFall(Vector3 fallDirection) 
        {
            Vector3 direction = new Vector3(fallDirection.x, 0, fallDirection.z).normalized;

            m_FallingTree.GetComponent<Collider>().enabled = true;
            m_FallingTree.isKinematic = false;
            m_FallingTree.AddForce(direction * m_TreeFallForce, ForceMode.Impulse);

            m_TreeFallAudio.Play(AudioSource, 1f, SelectionType.Random);

            if (m_ImpactTrigger != null)
                m_ImpactTrigger.onTriggerEnter += OnTreeImpact;

            m_TreeStump.enabled = true;
            m_TreeStump.transform.SetParent(transform.parent, true);

            m_IsFalling = true;
            m_HadImpact = false;
        }

        private void Awake()
        {
            m_FallingTree.isKinematic = true;
            m_FallingTree.GetComponent<Collider>().enabled = false;
            m_TreeStump.enabled = false;
        }

        private void Update()
        {
            if (!m_IsFalling || m_HadImpact)
                return;

            // Force start the tree impact behaviour when the time limit is up
            if (m_TimeSinceFallStart > m_MaxTimeToFall)
                OnTreeImpact(null);
            // Force start the tree impact behaviour if the velocity of the tree fall is ~0f
            else if (m_TimeSinceFallStart > m_MaxTimeToFall / 3f && m_FallingTree.velocity.sqrMagnitude == 0f)
                OnTreeImpact(null);

            m_TimeSinceFallStart += Time.deltaTime;
        }

        private void OnTreeImpact(Collider other)
        {
            if (!m_HadImpact && m_IsFalling)
                StartCoroutine(C_DelayedImpact());

            if (m_ImpactTrigger != null)
                m_ImpactTrigger.onTriggerEnter -= OnTreeImpact;
        }

        private IEnumerator C_DelayedImpact() 
        {
            m_TreeImpactAudio.Play(AudioSource, 1f, SelectionType.RandomExcludeLast);

            m_LogsRoot.SetActive(true);
            m_LogsRoot.transform.SetParent(null);

            CameraShakeHandler.DoShake(transform.position, m_CameraShake, 1f);

            yield return new WaitForSeconds(0.1f);

            foreach (var logRigidbody in m_LogsRoot.transform.GetComponentsInChildren<Rigidbody>())
            {
                Transform logTransform = logRigidbody.transform;
                logTransform.parent = null;

                logRigidbody.transform.position = new Vector3(logTransform.position.x, logTransform.position.y + m_LogsGroundOffset, logTransform.position.z);
                logRigidbody.AddForce(m_TreeImpactLogsForce * Random.Range(-1f, 1f) * logTransform.right, ForceMode.Impulse);

                if (m_TreeImpactFX != null)
                    Instantiate(m_TreeImpactFX, logTransform.position, logTransform.rotation);
            }

            m_HadImpact = true;
            m_IsFalling = false;

            if (m_DestroyType == DestroyType.ThisObject)
                Destroy(gameObject);
        }

        #region Save & Load
        public override void LoadMembers(object[] members)
        {
            m_IsFalling = (bool)members[0];
            m_HadImpact = (bool)members[1];
            m_HitDirection = (Vector3)members[2];

            if (m_IsFalling && !m_HadImpact)
                StartTreeFall(m_HitDirection);
        }

        public override object[] SaveMembers()
        {
            return new object[]
            {
                m_IsFalling,
                m_HadImpact,
                m_HitDirection
            };
        }
        #endregion
    }
}
