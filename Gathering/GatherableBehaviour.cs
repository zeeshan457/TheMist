using UnityEngine;

namespace SurvivalTemplatePro.ResourceGathering
{
    public abstract class GatherableBehaviour : MonoBehaviour
    {
        protected AudioSource AudioSource { get; private set; }
        protected IGatherable Gatherable { get; private set; }


        public void InitializeBehaviour(IGatherable gatherable, AudioSource audioSource)
        {
            this.Gatherable = gatherable;
            this.AudioSource = audioSource;
        }

        public virtual void DoHitEffects(DamageInfo damageInfo) { }
        public virtual void DoDestroyEffects(DamageInfo damageInfo) { }

        #region Save & Load
        public virtual void LoadMembers(object[] members) { }
        public virtual object[] SaveMembers() => null;
        #endregion
    }
}