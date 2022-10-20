using UnityEngine;

namespace SurvivalTemplatePro.ResourceGathering
{
    [CreateAssetMenu(menuName = "Survival Template Pro/Gathering/Gatherable Definition")]
    public class GatherableDefinition : ScriptableObject
    {
        public string Name => m_GatherableName;
        public Sprite Icon => m_Icon;
        public string Description => m_Description;

        [SerializeField]
        private string m_GatherableName;

        [SerializeField, Multiline]
        private string m_Description;

        [SerializeField, SpritePreview]
        private Sprite m_Icon;
    }
}