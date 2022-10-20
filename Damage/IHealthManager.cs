using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public interface IHealthManager : ICharacterModule
    {
        bool IsAlive { get; }

        float Health { get; }
        float PrevHealth { get; }
        float MaxHealth { get; set; }

        event UnityAction<DamageInfo> onDamageTaken;
        event UnityAction<float> onHealthChanged;
        event UnityAction<float> onHealthRestored;
        event UnityAction<float> onMaxHealthChanged;
        event UnityAction onDeath;
        event UnityAction onRespawn;

        void RestoreHealth(float healthRestore);
        void ReceiveDamage(DamageInfo damageInfo);
        void ReceiveDamage(float damage);
    }
}