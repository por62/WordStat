namespace WordStat6
{
	interface ILanguageDetector
	{
		Lang? GetLanguage(string text);
	}
}
