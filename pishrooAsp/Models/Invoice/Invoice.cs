using System.ComponentModel.DataAnnotations;

namespace pishrooAsp.Models.Invoice
{
	// Models/Invoice.cs
	public class Invoice
	{
		public int Id { get; set; }

		[Required]
		[StringLength(50)]
		public string TrackingCode { get; set; }

		[Required]
		public string FileName { get; set; }

		public string OriginalFileName { get; set; }

		[Required]
		public string CustomerName { get; set; }

		public string CustomerPhone { get; set; } // شماره تلفن برای پیامک

	

		public decimal Amount { get; set; }

		public DateTime InvoiceDate { get; set; } = DateTime.Now;

		public string Description { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime? SentDate { get; set; } // تاریخ ارسال پیامک

		[StringLength(10)]
		public string AccessToken { get; set; } // توکن دسترسی اضافی برای امنیت بیشتر
	}
}
