using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pishrooAsp.Models
{
	public class Catalog
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "عنوان کاتالوگ")]
		[Required(ErrorMessage = "عنوان کاتالوگ الزامی است")]
		[MaxLength(100, ErrorMessage = "عنوان نمی‌تواند بیشتر از 100 کاراکتر باشد")]
		public string Title { get; set; } = string.Empty;

		[Display(Name = "توضیحات")]
		[MaxLength(10000, ErrorMessage = "توضیحات نمی‌تواند بیشتر از 500 کاراکتر باشد")]
		public string? Description { get; set; }

		[Display(Name = "نام فایل")]
		[Required(ErrorMessage = "نام فایل الزامی است")]
		public string FileName { get; set; } = string.Empty;

		[Display(Name = "نام اصلی فایل")]
		public string? OriginalFileName { get; set; }

		[Display(Name = "مسیر فایل")]
		public string? FilePath { get; set; }

		[Display(Name = "حجم فایل")]
		public long FileSize { get; set; }

		[Display(Name = "تاریخ آپلود")]
		public DateTime UploadDate { get; set; } = DateTime.Now;

		[Display(Name = "فعال")]
		public bool IsActive { get; set; } = true;
	}
}