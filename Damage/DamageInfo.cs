using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// 
    /// </summary>
    public struct DamageInfo
	{
		/// <summary>  </summary>
		public float Damage;

		/// <summary>  </summary>
		public ICharacter Source;

		/// <summary>  </summary>
		public DamageType DamageType;

		/// <summary>  </summary>
		public Vector3 HitPoint;

		/// <summary>  </summary>
		public Vector3 HitDirection;

		/// <summary>  </summary>
		public float HitImpulse;


		public DamageInfo(float damage)
		{
			Damage = Mathf.Abs(damage);

			DamageType = DamageType.Generic;

			HitPoint = HitDirection = Vector3.zero;
			HitImpulse = 0f;

			Source = null;
		}

		public DamageInfo(float damage, ICharacter source)
		{
			Damage = Mathf.Abs(damage);

			DamageType = DamageType.Generic;

			HitPoint = Vector3.zero;
			HitDirection = Vector3.zero;
			HitImpulse = 0f;

			Source = source;
		}

		public DamageInfo(float damage, DamageType damageType, ICharacter source)
		{
			Damage = Mathf.Abs(damage);

			DamageType = damageType;

			HitPoint = Vector3.zero;
			HitDirection = Vector3.zero;
			HitImpulse = 0f;

			Source = source;
		}

		public DamageInfo(float damage, Vector3 hitPoint, Vector3 hitDirection, float hitImpulse, ICharacter source = null)
		{
			Damage = Mathf.Abs(damage);

			DamageType = DamageType.Generic;

			HitPoint = hitPoint;
			HitDirection = hitDirection;
			HitImpulse = hitImpulse;

			Source = source;
		}

		public DamageInfo(float damage, DamageType damageType, Vector3 hitPoint, Vector3 hitDirection, float hitImpulse, ICharacter source = null)
		{
			Damage = Mathf.Abs(damage);

			DamageType = damageType;

			HitPoint = hitPoint;
			HitDirection = hitDirection;
			HitImpulse = hitImpulse;

			Source = source;
		}
	}

	public enum DamageType
	{
		Generic,
		Cut,
		Hit,
		Stab,
		Bullet
	}
}