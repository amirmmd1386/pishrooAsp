namespace pishrooAsp.Models.Newses
{
	public class NewsTranslation
	{
		public int Id { get; set; }
		public int NewsId { get; set; }
		public int LangId { get; set; }
		public string Title { get; set; }
		public string Summary { get; set; }
		public string Content { get; set; }
		public string MetaKeywords { get; set; } // برای SEO
		public string MetaDescription { get; set; } // برای SEO

		// روابط
		public News News { get; set; }
		public Lang Lang { get; set; }
	}
}
