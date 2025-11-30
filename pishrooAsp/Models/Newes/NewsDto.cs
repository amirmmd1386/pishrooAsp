using pishrooAsp.Models.Newses;

namespace pishrooAsp.Models.Newes
{
	public class NewsDto
	{
		public int Id { get; set; }
		public bool IsPublished { get; set; }
		public int AuthorId { get; set; }
		public IFormFile DefaultImage { get; set; }
		public string DefaultImageUrl { get; set; }
		public List<NewsTranslation> Translations { get; set; } = new();
		public List<IFormFile> Images { get; set; }
		public List<NewsImage> ExistingImages { get; set; } = new();
		public List<IFormFile> Attachments { get; set; }
		public List<NewsAttachment> ExistingAttachments { get; set; } = new();
	}
	// در پوشه ModelViewers یا جایی مناسب
	public class NewsSidebarViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string DefaultImageUrl { get; set; }
		public DateTime PublishDate { get; set; }
	}

	public class ProductSidebarViewModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string ImageUrl { get; set; }
		public string SeoWord { get; set; }
	}
}
