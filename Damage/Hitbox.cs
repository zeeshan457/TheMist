using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Will register damage events from outside and pass them to the parent character.
    /// </summary>
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
	public class Hitbox : CharacterBehaviour, IDamageReceiver
	{
		public Collider Collider => m_Collider;
		public Rigidbody Rigidbody => m_Rigidbody;

		[SerializeField]
		private bool m_IsCritical;

		[SerializeField, Range(0f, 100f)]
		private float m_DamageMultiplier = 1f;

		private Collider m_Collider;
		private Rigidbody m_Rigidbody;
		private IHealthManager m_Health;


		public DamageResult HandleDamage(DamageInfo dmgInfo)
		{
			if (enabled && m_Health != null)
			{
                if (dmgInfo.Source != Character && m_Health.IsAlive)
                {
                    dmgInfo.Damage *= m_DamageMultiplier;

					m_Health.ReceiveDamage(dmgInfo);

					if (!m_Health.IsAlive)
						m_Rigidbody.AddForceAtPosition(dmgInfo.HitDirection * dmgInfo.HitImpulse, dmgInfo.HitPoint, ForceMode.Impulse);

					return m_IsCritical ? DamageResult.Critical : DamageResult.Default;
				}
			}
			
			return DamageResult.Ignored;
		}

        public override void OnInitialized()
        {
			m_Health = GetModule<IHealthManager>();

			if (m_Health == null)
				m_Health = GetComponentInParent<IHealthManager>();

			if (m_Health == null)
			{
				Debug.LogErrorFormat(this, "[This HitBox is not part of an entity, like a player, animal, etc, it has no purpose.", name);
				enabled = false;
				return;
			}

			m_Collider = GetComponent<Collider>();
			m_Rigidbody = GetComponent<Rigidbody>();
		}

#if UNITY_EDITOR
        private void Reset()
        {
			gameObject.layer = LayerMask.NameToLayer("Hitbox"); 
		}
#endif
    }
}