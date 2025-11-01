using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.ProductRequest
{
	public class ProductRequestFile
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		public Guid ProductRequestId { get; set; }

		public string FilePath { get; set; } // مسیر فایل آپلودی

		public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

		// Navigation Property
		public ProductRequest ProductRequest { get; set; }
	}
}
