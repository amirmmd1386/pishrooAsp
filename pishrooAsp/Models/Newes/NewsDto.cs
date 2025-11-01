using pishrooAsp.Models.Newses;

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
