using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    /// <summary>
    /// Base class for interactable objects (eg. Storage boxes, doors, item pickups).
    /// Has numerous raycast and interaction callbacks (overridable).
    /// </summary>
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/interaction/interactable")]
    public class Interactable : MonoBehaviour, IInteractable
    {
        #region Internal
        [System.Serializable]
        private class InteractEvent : UnityEvent<ICharacter> { }
        #endregion

        public virtual bool InteractionEnabled 
        {
            get => m_InteractionEnabled;
            set
            {
                if (value != m_InteractionEnabled)
                {
                    m_InteractionEnabled = value;
                    onInteractionEnabledChanged?.Invoke(value);
                }
            }
        }

        public string InteractionText 
        {
            get => m_InteractionText;
            protected set => m_InteractionText = value;
        }

        public string DescriptionText
        {
            get => m_Description;
            protected set
            {
                m_Description = value;
                onDescriptionTextChanged?.Invoke();
            }
        }

        public float HoldDuration => m_HoldDuration;
        public bool HoverActive => m_HoverActive;

        public event UnityAction onInteracted;
        public event UnityAction onDescriptionTextChanged;
        public event UnityAction<bool> onInteractionEnabledChanged;

        [Title("Settings (Interactable)")]

        [SerializeField]
        [Tooltip("Is this object interactable, if not, this object will be treated like a normal one.")]
        private bool m_InteractionEnabled = true;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How time it takes to interact with this object. (e.g. for how many seconds should the Player hold the interact button).")]
        private float m_HoldDuration = 0f;

        [SerializeField]
        [Tooltip("Interactable text (could be used as a name), shows up in the UI when looking at an object.")]
        private string m_InteractionText;

        [SerializeField, Multiline]
        [Tooltip("Interactable description, shows up in the UI when looking at an object.")]
        private string m_Description;

        [Space]

        [SerializeField]
        [Tooltip("Unity event that will be called when a character interacts with this object.")]
        private InteractEvent m_OnInteractCallback;

        [SerializeField, HideInInspector]
        protected MaterialChanger m_MaterialChanger;

        private bool m_HoverActive;


        /// <summary>
        /// Called when a character starts looking at this object.
        /// </summary>
        public virtual void OnHoverStart(ICharacter character)
        {
            if (!m_InteractionEnabled)
                return;

            if (m_MaterialChanger != null)
                m_MaterialChanger.SetMaterialWithEffects();

            m_HoverActive = true;
        }

        /// <summary>
        /// Called when a character stops looking at this object.
        /// </summary>
        public virtual void OnHoverEnd(ICharacter character)
        {
            if (!m_InteractionEnabled)
                return;

            if (m_MaterialChanger != null)
                m_MaterialChanger.SetDefaultMaterial();

            m_HoverActive = false;
        }

        /// <summary>
        /// Called when a character interacts with this object.
        /// </summary>
        public virtual void OnInteract(ICharacter character)
        {
            if (!m_InteractionEnabled)
                return;

            onInteracted?.Invoke();
            m_OnInteractCallback?.Invoke(character);
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            InteractionText = this.GetType().Name.ToUnityLikeNameFormat();
        }

        protected virtual void OnValidate()
        {
            if (m_MaterialChanger == null)
                m_MaterialChanger = transform.root.GetComponentInChildren<MaterialChanger>();
        }
#endif
    }
}