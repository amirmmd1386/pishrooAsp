namespace pishrooAsp.ModelViewer
{
	public class NewsViewModelSecond
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Summary { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public string ImageUrl { get; set; } = string.Empty;
		public DateTime PublishDate { get; set; }
		public string? Author { get; set; }
		public int ViewCount { get; set; }
		public string? Category { get; set; }
	}
	public class NewsViewModel
	{
		public int Id { get; set; }
		public DateTime PublishDate { get; set; }
		public bool IsPublished { get; set; }
		public int AuthorId { get; set; }
		public string DefaultImageUrl { get; set; }

		public List<NewsTranslationViewModel> Translations { get; set; }
		public List<NewsImageViewModel> Images { get; set; }
		public List<NewsAttachmentViewModel> Attachments { get; set; }
	}

	public class NewsTranslationViewModel
	{
		public int LangId { get; set; }
		public string Title { get; set; }
		public string Summary { get; set; }
		public string Content { get; set; }
		public string MetaKeywords { get; set; } // برای SEO
		public string MetaDescription { get; set; } // برای SEO
	}

	public class NewsImageViewModel
	{
		public string ImageUrl { get; set; }
	}

	public class NewsAttachmentViewModel
	{
		public string FileUrl { get; set; }
	}
}
