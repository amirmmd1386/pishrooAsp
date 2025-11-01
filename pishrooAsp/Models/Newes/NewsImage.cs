namespace pishrooAsp.Models.Newses
{
	public class NewsImage
	{
		public int Id { get; set; }
		public int NewsId { get; set; }
		public string ImageUrl { get; set; }
		public string AltText { get; set; } // متن جایگزین برای SEO
		public int DisplayOrder { get; set; } // ترتیب نمایش

		// روابط
		public News News { get; set; }
	}
}
