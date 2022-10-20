using UnityEngine;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Handles dealing fall damage to the character based on the impact velocity.
    /// </summary>
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/health#fall-damage-handler-behaviour")]
    public class FallDamageHandler : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableDamage = true;

        [Space]

        [InfoBox("At which landing speed, the character will start taking damage.")]
        [SerializeField, Range(1f, 30f)] 
        private float m_MinFallSpeed = 12f;

        [Space]

        [InfoBox("At which landing speed, the character will take maximum damage (die).")]
        [SerializeField, Range(1f, 50f)]
        private float m_FatalFallSpeed = 30f;


        public override void OnInitialized()
        {
            GetModule<ICharacterMotor>().onFallImpact += OnFallImpact;
        }

        private void OnFallImpact(float impactSpeed)
        {
            if (!m_EnableDamage)
                return;

            if (impactSpeed >= m_MinFallSpeed)
                Character.HealthManager.ReceiveDamage(new DamageInfo(-100f * (impactSpeed / m_FatalFallSpeed)));
        }
    }
}