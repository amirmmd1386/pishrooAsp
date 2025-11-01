namespace pishrooAsp.ModelViewer
{
	public class ProductCreateViewModel
	{
		public IFormFile? Image { get; set; }
		public IFormFile? Attachment { get; set; }

		public List<ProductTranslationViewModel> Translations { get; set; } = new();
	}
	public class ProductTranslationViewModel
	{
		public Guid Id { get; set; }
		public string? Name { get; set; } // فقط برای نمایش
		public int LangId { get; set; }   // ✅ این باید int باشه
		public string Title { get; set; }
		public string? Description { get; set; }
	}

}
