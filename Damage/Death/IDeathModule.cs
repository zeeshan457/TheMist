namespace SurvivalTemplatePro
{
    public interface IDeathModule : ICharacterModule
    {
        void DoDeathEffects(ICharacter character);
        void DoRespawnEffects(ICharacter character);
    }
}