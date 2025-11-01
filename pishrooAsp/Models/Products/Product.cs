using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.Products
{
	public class Product
	{
		public Guid Id { get; set; }

		public string? ImageUrl { get; set; }

		public string? AttachmentUrl { get; set; }

		public string? seoWord { get; set; }
		public string? SeoDiscription { get; set; }


		// 4 کاربرد مختلف برای محصول
		public string? Usage1FileUrl { get; set; }
		public string? Usage1Title { get; set; }
		public string? Usage1FileType { get; set; } // pdf, image

		public string? Usage2FileUrl { get; set; }
		public string? Usage2Title { get; set; }
		public string? Usage2FileType { get; set; }

		public string? Usage3FileUrl { get; set; }
		public string? Usage3Title { get; set; }
		public string? Usage3FileType { get; set; }

		public string? Usage4FileUrl { get; set; }
		public string? Usage4Title { get; set; }
		public string? Usage4FileType { get; set; }

		public string? Usage5FileUrl { get; set; } // کاربرد پنجم اختیاری
		public string? Usage5Title { get; set; }
		public string? Usage5FileType { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<ProductTranslation>? Translations { get; set; }
	}
}