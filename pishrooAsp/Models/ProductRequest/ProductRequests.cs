using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pishrooAsp.Models.ProductRequest
{
	public enum RequestStatus
	{
		Pending = 0,   // در انتظار بررسی
		Checked = 1,   // بررسی شد
		Rejected = 2   // رد شد
	}
	public class ProductRequest
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required, MaxLength(100)]
		public string FullName { get; set; } // نام و نام خانوادگی

		[Required, MaxLength(20)]
		public string PhoneNumber { get; set; } // شماره تماس

		[MaxLength(150)]
		public string CompanyName { get; set; } // نام شرکت یا محل کار

		[MaxLength(20)]
		public string CompanyPhone { get; set; } // تلفن محل کار

		[MaxLength(50)]
		public string InjectionMethod { get; set; } // روش تزریق (سه مدل)

		[MaxLength(100)]
		public string ProductType { get; set; } // نوع محصول مورد نیاز

		public double? RequiredAmountKg { get; set; } // مقدار مورد نیاز به کیلوگرم

		public int? SoftnessA { get; set; } // نرمی A (0 تا 90)
		public int? HardnessB { get; set; } // سفتی B (10 تا 50)

		//	public string? FilePath { get; set; } // مسیر فایل آپلود شده

		public ICollection<ProductRequestFile>? Files { get; set; } = new List<ProductRequestFile>();

		// ✅ برای گرفتن فایل‌ها از فرم
		[NotMapped]
		public List<IFormFile>? UploadFiles { get; set; }

		public string? Description { get; set; } // توضیحات

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public RequestStatus Status { get; set; } = RequestStatus.Pending;

		[Display(Name = "تاریخ آخرین پیام ارسالی")]
		public DateTime? LastMessageSentAt { get; set; }
	


	}
}
