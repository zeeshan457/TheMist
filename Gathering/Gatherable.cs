using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.ResourceGathering
{
    [RequireComponent(typeof(Collider), typeof(AudioSource))]
    public class Gatherable : MonoBehaviour, IGatherable, ISaveableComponent
    {
        #region Internal
        private struct BehavioursData
        {
            public string BehaviourName;
            public object[] Members;
        }
        #endregion

        public GatherableDefinition Definition => m_Definition;
        public float Health => m_Health;
        public float MaxHealth => m_MaxHealth;
        public float GatherRadius => m_GatherRadius;
        public Vector3 GatherOffset => m_GatherOffset;

        [SerializeField]
        private GatherableDefinition m_Definition;

        [SerializeField, InfoBox("Prefab to SPAWN when a character interacts with this gatherable.")]
        private GatherableBehaviour m_RiggedPrefab;

        [Space]

        [SerializeField, InfoBox("Visuals to DISABLE when a character interacts with this gatherable.")]
        private GameObject m_BaseVisuals;

        [Title("Health")]

        [SerializeField, Range(0f, 1000f)]
        private float m_MaxHealth = 100f;

        [SerializeField, Range(0f, 1000f)]
        private float m_InitialHealth = 100f;

        [Title("Gathering")]

        [SerializeField]
        private Vector3 m_GatherOffset;

        [SerializeField, Range(0.1f, 1f)]
        private float m_GatherRadius = 0.35f;

        private GatherableBehaviour[] m_Behaviours;

        private float m_Health;
        private bool m_BehavioursInitialized;

        private Collider m_Collider;
        private AudioSource m_AudioSource;

        private BehavioursData[] m_BehavioursData;
        private static readonly Dictionary<GatherableDefinition, List<Gatherable>> m_AllGatherables = new Dictionary<GatherableDefinition, List<Gatherable>>();


        #region Static Methods

        public static bool TryGetAllGatherablesWithDefinition(GatherableDefinition def, out List<Gatherable> gatherables)
        {
            return m_AllGatherables.TryGetValue(def, out gatherables);
        }

        private static void RegisterGatherable(Gatherable gatherable)
        {
            if (m_AllGatherables.TryGetValue(gatherable.Definition, out List<Gatherable> gatherables))
            {
                gatherables.Add(gatherable);
            }
            else if (gatherable.Definition != null)
            {
                List<Gatherable> gatherableList = new List<Gatherable>() { gatherable };
                m_AllGatherables.Add(gatherable.Definition, gatherableList);
            }
        }

        private static void UnregisterGatherable(Gatherable gatherable)
        {
            if (m_AllGatherables.TryGetValue(gatherable.Definition, out List<Gatherable> gatherableList))
                gatherableList.Remove(gatherable);
        }

        #endregion

        public virtual DamageResult Damage(DamageInfo dmgInfo)
        {
            if (m_Health < 0.001f || dmgInfo.Damage < 0.01f)
                return DamageResult.Ignored;

            if (!m_BehavioursInitialized)
                SpawnBehaviours();

            DamageGatherable(dmgInfo);

            if (m_Health < 1f)
            {
                DestroyGatherable(dmgInfo);
                return DamageResult.Critical;
            }

            return DamageResult.Default;
        }

        public virtual void ResetHealth()
        {
            if (m_Health - m_InitialHealth < 0.1f)
                return;

            if (m_BehavioursInitialized)
            {
                for (int i = 0; i < m_Behaviours.Length; i++)
                {
                    if (m_Behaviours[i] != null)
                        Destroy(m_Behaviours[i].gameObject);
                }

                m_Behaviours = null;
                m_BehavioursInitialized = false;
            }

            m_BaseVisuals.SetActive(true);
        }

        protected virtual void DamageGatherable(DamageInfo dmgInfo)
        {
            m_Health = Mathf.Clamp(m_Health - dmgInfo.Damage, 0f, 100f);

            for (int i = 0; i < m_Behaviours.Length; i++)
                m_Behaviours[i].DoHitEffects(dmgInfo);
        }

        protected virtual void DestroyGatherable(DamageInfo dmgInfo) 
        {
            m_Collider.enabled = false;

            for (int i = 0; i < m_Behaviours.Length; i++)
                m_Behaviours[i].DoDestroyEffects(dmgInfo);
        }

        protected virtual void SpawnBehaviours()
        {
            var behaviourObject = Instantiate(m_RiggedPrefab, transform);
            m_Behaviours = behaviourObject.GetComponents<GatherableBehaviour>();

            for (int i = 0; i < m_Behaviours.Length; i++)
                m_Behaviours[i].InitializeBehaviour(this, m_AudioSource);

            m_BehavioursInitialized = true;
            m_BaseVisuals.SetActive(false);
        }

        protected virtual void Start()
        {
            m_Health = m_InitialHealth;

            m_Collider = GetComponent<Collider>();
            m_AudioSource = GetComponent<AudioSource>();

            m_Collider.isTrigger = false;
        }

        private void Awake() => RegisterGatherable(this);
        private void OnDestroy() => UnregisterGatherable(this);

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_BehavioursInitialized = (bool)members[0];
            m_Health = (float)members[1];

            m_BehavioursData = members[2] as BehavioursData[];

            if (m_BehavioursInitialized)
            {
                SpawnBehaviours();

                for (int i = 0; i < m_BehavioursData.Length; i++)
                {
                    var behaviour = GetBehaviourWithName(m_BehavioursData[i].BehaviourName);

                    if (behaviour != null)
                        behaviour.LoadMembers(m_BehavioursData[i].Members);
                }
            }

            if (m_Health < 1f)
                m_Collider.enabled = false;
        }

        public object[] SaveMembers()
        {
            if (m_BehavioursInitialized)
            {
                m_BehavioursData = new BehavioursData[m_Behaviours.Length];

                for (int i = 0; i < m_BehavioursData.Length; i++)
                {
                    m_BehavioursData[i] = new BehavioursData()
                    {
                        BehaviourName = m_Behaviours[i].GetType().Name,
                        Members = m_Behaviours[i].SaveMembers(),
                    };
                }
            }

            return new object[]
            {
                m_BehavioursInitialized,
                m_Health,
                m_BehavioursData
            };
        }

        private GatherableBehaviour GetBehaviourWithName(string name) 
        {
            for (int i = 0; i < m_Behaviours.Length; i++)
            {
                if (m_Behaviours[i].GetType().Name == name)
                    return m_Behaviours[i];
            }

            return null;
        }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_AudioSource != null)
                m_AudioSource.spatialBlend = 1f;
        }

        private void OnDrawGizmosSelected()
        {
            if (Event.current.type == EventType.Repaint)
            {
                Vector3 gatherPoint = transform.position + transform.TransformVector(m_GatherOffset);

                UnityEditor.Handles.CircleHandleCap(0, gatherPoint, Quaternion.LookRotation(Vector3.up), m_GatherRadius, EventType.Repaint);

                UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.5f);
                UnityEditor.Handles.SphereHandleCap(0, gatherPoint, Quaternion.identity, 0.1f, EventType.Repaint);
                UnityEditor.Handles.color = Color.white;

                UnityEditor.Handles.Label(gatherPoint, "Gather Position");
            }
        }
#endif
    }
}