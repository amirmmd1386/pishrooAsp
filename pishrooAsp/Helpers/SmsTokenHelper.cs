using System.Text.RegularExpressions;

public static class TokenHelper
{
	public static List<string> ExtractTokens(string text)
	{
		var matches = Regex.Matches(text, @"\{(.*?)\}");
		return matches.Select(m => m.Groups[1].Value).Distinct().ToList();
	}
}
