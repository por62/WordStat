namespace WordStat.Core
{
	interface ILanguageDetector
	{
		Lang? GetLanguage(string text);
	}
}
