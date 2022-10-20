using UnityEngine;

namespace SurvivalTemplatePro.ResourceGathering
{
    public interface IGatherable
    {
        GatherableDefinition Definition { get; }

        float Health { get; }
        float MaxHealth { get; }
        float GatherRadius { get; }
        Vector3 GatherOffset { get; }

        void ResetHealth();
        DamageResult Damage(DamageInfo dmgInfo);

        #region Monobehaviour
        GameObject gameObject { get; }
        Transform transform { get; }
        #endregion
    }
}