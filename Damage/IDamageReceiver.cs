namespace SurvivalTemplatePro
{
	public interface IDamageReceiver
	{
		DamageResult HandleDamage(DamageInfo damageInfo);
	}

	public enum DamageResult
	{
		Default,
		Critical,
		Blocked,
		Reflected,
		Ignored
	}
}