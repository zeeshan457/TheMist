namespace SurvivalTemplatePro
{
    public interface ISTPEventHandler
    {
        void TriggerAction(string name, float value = 1f);
        void TriggerAction(STPEventReference eventReference, float value = 1f);
        bool TryGetEventWithName(string name, out STPEvent stpEvent);

#if UNITY_EDITOR
        string[] GetAllEventNames();
#endif
    }
}