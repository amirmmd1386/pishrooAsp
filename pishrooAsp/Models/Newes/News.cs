namespace pishrooAsp.Models.Newses
{
	public class News
	{
		public int Id { get; set; }
		public DateTime PublishDate { get; set; }
		public bool IsPublished { get; set; }
		public int? AuthorId { get; set; } // نویسنده خبر (اختیاری)
		public string DefaultImageUrl { get; set; } // تصویر پیش‌فرض خبر

		// روابط
		public List<NewsTranslation> Translations { get; set; } = new();
		public List<NewsImage>? Images { get; set; }	
		public List<NewsAttachment>? Attachments { get; set; }
	}
}
