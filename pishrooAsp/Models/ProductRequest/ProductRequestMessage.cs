using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.ProductRequest
{
	public class ProductRequestMessage
	{
		public int Id { get; set; }
		public Guid ProductRequestId { get; set; }
		public ProductRequest ProductRequest { get; set; }

		public int MessageId { get; set; }
		public message Message { get; set; }

		public DateTime SentAt { get; set; } = DateTime.UtcNow; // زمان ارسال پیام

		[MaxLength(20)]
		public string? Status { get; set; } // وضعیت ارسال (Sent, Failed, etc.)
	}
}
