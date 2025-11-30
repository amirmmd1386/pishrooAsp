// در پوشه Utilities/SeoHelper.cs
namespace pishrooAsp.Utilities
{
	public static class SeoHelper
	{
		public static string GenerateSeoTitle(string title)
		{
			if (string.IsNullOrEmpty(title))
				return null;

			var seoTitle = title.ToLower()
				.Replace(" ", "-")
				.Replace("_", "-")
				.Replace(".", "")
				.Replace(",", "")
				.Replace(":", "")
				.Replace(";", "")
				.Replace("!", "")
				.Replace("?", "")
				.Replace("(", "")
				.Replace(")", "")
				.Replace("[", "")
				.Replace("]", "")
				.Replace("{", "")
				.Replace("}", "")
				.Replace("@", "")
				.Replace("#", "")
				.Replace("$", "")
				.Replace("%", "")
				.Replace("^", "")
				.Replace("&", "")
				.Replace("*", "")
				.Replace("+", "")
				.Replace("=", "")
				.Replace("|", "")
				.Replace("\\", "")
				.Replace("/", "")
				.Replace("\"", "")
				.Replace("'", "")
				.Replace("`", "")
				.Replace("~", "");

			while (seoTitle.Contains("--"))
			{
				seoTitle = seoTitle.Replace("--", "-");
			}

			seoTitle = seoTitle.Trim('-');

			return seoTitle;
		}
	}
}