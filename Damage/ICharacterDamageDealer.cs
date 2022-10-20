using UnityEngine;

namespace SurvivalTemplatePro
{
    public interface ICharacterDamageDealer : ICharacterModule
    {
        event OnDamageDealtCallback onDamageDealt;

        public DamageResult DealDamage(Collider damageable, float damage, DamageType damageType = DamageType.Generic, Vector3 hitPoint = default, Vector3 hitDirection = default, float hitImpulse = 1f, ICharacter source = null);
	}

    public delegate void OnDamageDealtCallback(DamageResult damageResult);
}
